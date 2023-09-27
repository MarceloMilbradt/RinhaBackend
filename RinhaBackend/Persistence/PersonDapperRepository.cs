using Dapper;
using Npgsql;
using NpgsqlTypes;
using RinhaBackend.Models;
using System.Data;

namespace RinhaBackend.Persistence;

public class StringArrayHandler : SqlMapper.TypeHandler<string[]>
{
    public override string[] Parse(object value)
    {
        if (value is string stringValue)
        {
            return stringValue.Split(',');
        }
        return [];
    }

    public override void SetValue(IDbDataParameter parameter, string[] value)
    {
        parameter.Value = string.Join(',', value);
        parameter.DbType = DbType.String;
    }
}

internal sealed class PersonDapperRepository(IConfiguration configuration) : IPersonRepository
{
    private readonly string connectionString = configuration.GetConnectionString("rinhabackend");
    private async ValueTask<NpgsqlConnection> GetOrCreateAndOpenConnectionAsync(CancellationToken token)
    {
        SqlMapper.AddTypeHandler(new StringArrayHandler());
        var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync(token);
        return conn;
    }

    private const string GetByIdSQL = "SELECT \"Id\", \"Apelido\", \"Nome\", \"Nascimento\", \"Stack\" FROM public.\"Persons\" WHERE \"Id\" = @Id";
    private const string SqlBulkInsert = "COPY public.\"Persons\" (\"Id\", \"Apelido\", \"Nome\", \"Nascimento\", \"Stack\") FROM STDIN (FORMAT BINARY)";
    private const string SqlInsert = "INSERT INTO public.persons\r\n(id, apelido, nome, nascimento, stack)\r\nVALUES(@Id,@Apelido, @Nome, @Nascimento, @Stack);";
    private const string SearchTermQuery = """
        SELECT "Id", "Apelido", "Nome", "Nascimento", "Stack"
        FROM public."Persons" 
        WHERE "SearchField" ILIKE @term
        LIMIT 50
        """;

    public async Task BulkInsertAsync(IEnumerable<Person> items, CancellationToken cancellationToken)
    {
        using var connection = await GetOrCreateAndOpenConnectionAsync(cancellationToken);
        using var writer = connection.BeginBinaryImport(SqlBulkInsert);

        foreach (var item in items)
        {
            await writer.StartRowAsync(cancellationToken);
            await writer.WriteAsync(item.Id, NpgsqlDbType.Uuid, cancellationToken);
            await writer.WriteAsync(item.Apelido, NpgsqlDbType.Text, cancellationToken);
            await writer.WriteAsync(item.Nome, NpgsqlDbType.Text, cancellationToken);
            await writer.WriteAsync(item.Nascimento, NpgsqlDbType.Date, cancellationToken);
            await writer.WriteAsync(string.Join(',', item.Stack), NpgsqlDbType.Text, cancellationToken);
        }

        await writer.CompleteAsync(cancellationToken);
    }

    public async Task Create(Person person, CancellationToken token)
    {
        using var connection = await GetOrCreateAndOpenConnectionAsync(token);
        await connection.ExecuteAsync(SqlInsert, person);
    }
    public async Task<List<Person>> GetByTermAsync(string term, CancellationToken token)
    {
        using var connection = await GetOrCreateAndOpenConnectionAsync(token);
        var persons = await connection.QueryAsync<Person>(SearchTermQuery, new { term = $"%{term}%" });
        return persons.ToList();
    }

    public async Task<Person?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        using var connection = await GetOrCreateAndOpenConnectionAsync(cancellationToken);
        return await connection.QueryFirstOrDefaultAsync<Person>(GetByIdSQL, new { id });
    }

    public void ExecuteSql(string sql)
    {
        var conn = new NpgsqlConnection(connectionString);
        conn.Open();
        conn.Execute(sql);
    }

    public async Task<int> Count()
    {
        var conn = await GetOrCreateAndOpenConnectionAsync(default);
        return await conn.ExecuteScalarAsync<int>("select count(*) from \"Persons\"");
    }

}
