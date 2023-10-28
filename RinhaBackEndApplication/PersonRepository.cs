using Npgsql;
using NpgsqlTypes;
using System.Runtime.CompilerServices;
using System.Text;
namespace RinhaBackend.Application;

public sealed class PersonRepository(RedisCacheService cacheService)
{
    private const string IdParameterName = "Id";
    private const string ApelidoParameterName = "Apelido";
    private const string NomeParameterName = "Nome";
    private const string NascimentoParameterName = "Nascimento";
    private const string StackParameterName = "Stack";
    private const string TermParameterName = "Term";
    private const string ValueTemplate = "%{0}$";
    private static readonly NpgsqlDataSource _dataSource = NpgsqlDataSource.Create(Sql.ConnectionString);

    public async Task<Person> GetByIdAsync(Guid id, CancellationToken token)
    {
        var person = await cacheService.GetItemAsync(id);
        return person ?? await GetByIdFromDbAsync(id, token);
    }

    internal static async Task<Person> GetByIdFromDbAsync(Guid id, CancellationToken token)
    {
        //using var datasource = NpgsqlDataSource.Create(Sql.ConnectionString);
        using var connection = await _dataSource.OpenConnectionAsync(token);
        await using var cmd = new NpgsqlCommand(Sql.GetByIdSQL, connection);
        var param = cmd.Parameters.Add(IdParameterName, NpgsqlDbType.Uuid);
        if (!cmd.IsPrepared)
        {
            await cmd.PrepareAsync(token);
        }
        param.Value = id;
        await using var reader = await cmd.ExecuteReaderAsync(token);
        await reader.ReadAsync(token);
        return ReadPerson(reader);
    }
    public static async IAsyncEnumerable<Person> SearchPersonsAsync(string term, [EnumeratorCancellation] CancellationToken token)
    {
        //using var datasource = NpgsqlDataSource.Create(Sql.ConnectionString);
        using var connection = await _dataSource.OpenConnectionAsync(token);
        using var cmd = new NpgsqlCommand(Sql.SearchTermQuery, connection);
        var termParam = cmd.Parameters.Add(TermParameterName, NpgsqlDbType.Varchar);
        if (!cmd.IsPrepared)
        {
            await cmd.PrepareAsync(token);
        }
        termParam.Value = term;
        using var reader = await cmd.ExecuteReaderAsync(token);

        while (await reader.ReadAsync(token))
        {
            yield return ReadPerson(reader);
        }
    }

    private static Person ReadPerson(NpgsqlDataReader reader)
    {
        return new Person
        {
            Id = reader.GetGuid(0),
            Apelido = reader.GetString(1),
            Nome = reader.GetString(2),
            Nascimento = reader.GetDateTime(3),
            Stack = reader.GetString(4).Split(',')
        };
    }

    public static async Task<int> CountPersonsAsync()
    {
        //using var datasource = NpgsqlDataSource.Create(Sql.ConnectionString);
        using var connection = await _dataSource.OpenConnectionAsync();
        return Convert.ToInt32(await new NpgsqlCommand("SELECT COUNT(*) FROM public.\"Persons\"", connection).ExecuteScalarAsync());
    }

    internal static async Task BulkInsertAsync(IEnumerable<Person> items, CancellationToken cancellationToken)
    {
        using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        using var writer = await connection.BeginBinaryImportAsync(Sql.SqlBulkInsert, cancellationToken);

        var apelidoBuffer = new StringBuilder(32);
        var nomeBuffer = new StringBuilder(100);

        foreach (var person in items)
        {
            await writer.StartRowAsync(cancellationToken);
            await writer.WriteAsync(person.Id, NpgsqlDbType.Uuid, cancellationToken);

            apelidoBuffer.Clear().Append(TruncateString(person.Apelido, 32));
            await writer.WriteAsync(apelidoBuffer.ToString(), NpgsqlDbType.Text, cancellationToken);

            nomeBuffer.Clear().Append(TruncateString(person.Nome, 100));
            await writer.WriteAsync(nomeBuffer.ToString(), NpgsqlDbType.Text, cancellationToken);

            await writer.WriteAsync(person.Nascimento, NpgsqlDbType.Date, cancellationToken);
            await writer.WriteAsync(string.Join(",", person.Stack), NpgsqlDbType.Text, cancellationToken);
        }

        await writer.CompleteAsync(cancellationToken);
    }


    public static string TruncateString(string input, int maxLength)
    {
        return input.Length <= maxLength ? input : input[..maxLength];
    }
}