using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RinhaBackend.Migrations
{
    /// <inheritdoc />
    public partial class ChangeSearchTermCase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("CREATE EXTENSION IF NOT EXISTS unaccent;");
            migrationBuilder.Sql(
                """
                    CREATE OR REPLACE FUNCTION immutable_unaccent(text) RETURNS text AS
                    $$
                    SELECT unaccent($1);
                    $$
                    LANGUAGE SQL IMMUTABLE;
                    """);
            migrationBuilder.AlterColumn<string>(
                name: "SearchField",
                table: "Persons",
                type: "text",
                nullable: false,
                computedColumnSql: " immutable_unaccent(lower(\"Apelido\")) || ' ' || \r\n immutable_unaccent(lower(\"Nome\")) || ' ' || \r\n immutable_unaccent(lower(array_to_string_immutable(\"Stack\", ' ')))",
                stored: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldComputedColumnSql: "\"Apelido\" || ' ' || \"Nome\" || ' ' || array_to_string_immutable(\"Stack\", ' ')",
                oldStored: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP FUNCTION immutable_unaccent(text);");
            migrationBuilder.Sql("DROP EXTENSION IF EXISTS unaccent;");
            migrationBuilder.AlterColumn<string>(
                name: "SearchField",
                table: "Persons",
                type: "text",
                nullable: false,
                computedColumnSql: "\"Apelido\" || ' ' || \"Nome\" || ' ' || array_to_string_immutable(\"Stack\", ' ')",
                stored: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldComputedColumnSql: " immutable_unaccent(lower(\"Apelido\")) || ' ' || \r\n immutable_unaccent(lower(\"Nome\")) || ' ' || \r\n immutable_unaccent(lower(array_to_string_immutable(\"Stack\", ' ')))",
                oldStored: true);
        }
    }
}
