using Npgsql;
using NpgsqlTypes;
using RinhaBackEndNativeAot.Cache;
using RinhaBackEndNativeAot.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace RinhaBackEndNativeAot.Persistence;

internal sealed class PersonRepository(RedisCacheService redisCacheService)
{
    private const string GetByIdSQL = "SELECT \"Id\", \"Apelido\", \"Nome\", \"Nascimento\", \"Stack\" FROM public.\"Persons\" WHERE \"Id\" = @Id";

    private const string InsertSQL = """
            INSERT INTO public."Persons"("Id", "Apelido", "Nome", "Nascimento", "Stack", "SearchField")
            VALUES (@Id, @Apelido, @Nome, @Nascimento, @Stack, @SearchField);
        """;
    private const string SearchTermQuery = """
            SELECT "Id", "Apelido", "Nome", "Nascimento", "Stack"
            FROM public."Persons" 
             WHERE "SearchField" LIKE @Term 
             LIMIT 50
            """;
    private const string SqlBulkInsert = "COPY public.\"Persons\" (\"Id\", \"Apelido\", \"Nome\", \"Nascimento\", \"Stack\", \"SearchField\") FROM STDIN (FORMAT BINARY)";
    private static readonly string ConnectionString = "Host=db;Database=rinhabackend;Username=rinhabackend;Password=rinhabackend;Pooling=true;MaxPoolSize=100;MinPoolSize=10;CommandTimeout=60";

    private static async Task<NpgsqlConnection> CreateAndOpenConnectionAsync(CancellationToken token)
    {
        var conn = new NpgsqlConnection(ConnectionString);
        await conn.OpenAsync(token);
        return conn;
    }

    internal static async Task<IEnumerable<Person>> SearchPersonsAsync(string term, CancellationToken token)
    {
        await using var conn = await CreateAndOpenConnectionAsync(token);
        await using var cmd = new NpgsqlCommand(SearchTermQuery, conn);
        cmd.Parameters.AddWithValue("Term", $"%{term}%");

        var persons = new List<Person>(50);
        await using var reader = await cmd.ExecuteReaderAsync(token);
        while (await reader.ReadAsync(token))
        {
            persons.Add(ReadPerson(reader));
        }

        return persons;
    }

    internal async Task<Person> GetByIdAsync(Guid id, CancellationToken token)
    {
        var person = await redisCacheService.GetItemAsync(id);
        return person ?? await GetByIdFromDbAsync(id, token);
    }

    internal static async Task<Person> GetByIdFromDbAsync(Guid id, CancellationToken token)
    {
        await using var conn = await CreateAndOpenConnectionAsync(token);
        await using var cmd = new NpgsqlCommand(GetByIdSQL, conn);
        cmd.Parameters.AddWithValue("Id", id);

        await using var reader = await cmd.ExecuteReaderAsync(token);
        await reader.ReadAsync(token);
        return ReadPerson(reader);
    }

    private static Person ReadPerson(NpgsqlDataReader reader)
    {
        return new Person
        {
            Id = reader.GetGuid(0),
            Apelido = reader.GetString(1),
            Nome = reader.GetString(2),
            Nascimento = DateOnly.FromDateTime(reader.GetDateTime(3)),
            Stack = [.. reader.GetString(4).Split(',')]
        };
    }

    internal static async Task Create(Person person, CancellationToken token)
    {
        await using var conn = await CreateAndOpenConnectionAsync(token);
        await using var cmd = new NpgsqlCommand(InsertSQL, conn);

        cmd.Parameters.AddRange(new[]
        {
            new NpgsqlParameter("Id", person.Id),
            new NpgsqlParameter("Apelido", person.Apelido ?? (object)DBNull.Value),
            new NpgsqlParameter("Nome", person.Nome ?? (object)DBNull.Value),
            new NpgsqlParameter("Nascimento", person.Nascimento),
            new NpgsqlParameter("Stack", string.Join(",", person.Stack)),
            new NpgsqlParameter("SearchField", person.SearchField ?? (object)DBNull.Value)
        });

        await cmd.ExecuteNonQueryAsync(token);
    }

    internal static async Task<int> CountPersonsAsync()
    {
        await using var conn = await CreateAndOpenConnectionAsync(CancellationToken.None);
        return Convert.ToInt32(await new NpgsqlCommand(@"SELECT COUNT(*) FROM public.""Persons""", conn).ExecuteScalarAsync());
    }


    
}
