using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RinhaBackend.Migrations
{
    /// <inheritdoc />
    public partial class CreateDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("CREATE IF NOT EXISTS EXTENSION pg_trgm;");
            migrationBuilder.CreateTable(
                name: "Persons",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Apelido = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true), // Nullable and changed size
                    Nome = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false), // Changed size
                    Nascimento = table.Column<DateOnly>(type: "date", nullable: false), // Changed type
                    Stack = table.Column<List<string>>(type: "text[]", nullable: false),
                    SearchField = table.Column<string>(type: "varchar(500)", nullable: true) // Nullable and changed size
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Persons", x => x.Id);
                });
            migrationBuilder.Sql("CREATE INDEX \"IX_Persons_Id\" ON public.\"Persons\" USING btree (\"Id\");");
            migrationBuilder.Sql("CREATE INDEX idx_searchfield_trgm_gin ON public.\"Persons\" USING gin (\"SearchField\" gin_trgm_ops);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop indices
            migrationBuilder.Sql("DROP INDEX IF EXISTS idx_searchfield_trgm_gin;");
            migrationBuilder.Sql("DROP INDEX IF EXISTS \"IX_Persons_Id\";");

            // Drop table
            migrationBuilder.DropTable(
                name: "Persons");

            // Drop the pg_trgm extension
            migrationBuilder.Sql("DROP EXTENSION IF EXISTS pg_trgm;");
        }

    }
}
