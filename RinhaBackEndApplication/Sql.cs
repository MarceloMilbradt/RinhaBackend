namespace RinhaBackend.Application;

internal static class Sql
{
    public const string GetByIdSQL = "SELECT \"Id\", \"Apelido\", \"Nome\", \"Nascimento\", \"Stack\" FROM public.\"Persons\" WHERE \"Id\" = @Id";

    public const string InsertSQL = """
            INSERT INTO public."Persons"("Id", "Apelido", "Nome", "Nascimento", "Stack")
            VALUES (@Id, @Apelido, @Nome, @Nascimento, @Stack);
        """;
    public const string SearchTermQuery = """
            SELECT "Id", "Apelido", "Nome", "Nascimento", "Stack"
            FROM public."Persons" 
             WHERE "SearchField" ~* @Term 
             LIMIT 50
            """;
    public const string SqlBulkInsert = "COPY public.\"Persons\" (\"Id\", \"Apelido\", \"Nome\", \"Nascimento\", \"Stack\") FROM STDIN (FORMAT BINARY)";

#if DEBUG
    public static readonly string ConnectionString = "Host=localhost;Database=rinhabackend;Username=rinhabackend;Password=rinhabackend;Pooling=true;MaxPoolSize=1200;ConnectionIdleLifetime=45;Timeout=60;CommandTimeout=15;MaxAutoPrepare=10;";
#else
    public static readonly string ConnectionString = "Host=localhost;Database=rinhabackend;Username=rinhabackend;Password=rinhabackend;Pooling=true;MaxPoolSize=1200;ConnectionIdleLifetime=45;Timeout=60;CommandTimeout=15;MaxAutoPrepare=10;";
#endif
}
