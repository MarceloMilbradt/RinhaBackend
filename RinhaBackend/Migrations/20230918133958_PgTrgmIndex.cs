using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RinhaBackend.Migrations
{
    /// <inheritdoc />
    public partial class PgTrgmIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("CREATE EXTENSION pg_trgm;");
            migrationBuilder.Sql("CREATE INDEX idx_searchfield_trgm_gin ON \"Persons\" USING GIN (\"SearchField\" gin_trgm_ops);");
            migrationBuilder.DropIndex(
                name: "IX_Persons_SearchField",
                table: "Persons");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP EXTENSION IF EXISTS pg_trgm;");
            migrationBuilder.Sql("DROP INDEX idx_searchfield_trgm_gin;");
            migrationBuilder.CreateIndex(
                name: "IX_Persons_SearchField",
                table: "Persons",
                column: "SearchField");
        }
    }
}
