using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RinhaBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddDateInSearchTerm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                CREATE OR REPLACE FUNCTION generate_search_field(p_apelido varchar, p_nome varchar, p_nascimento date, p_stack text[])
                RETURNS text AS $$
                BEGIN
                    RETURN immutable_unaccent(lower(p_apelido)) || ' ' ||
                           immutable_unaccent(lower(p_nome)) || ' ' ||
                           p_nascimento || ' ' ||
                           immutable_unaccent(lower(array_to_string_immutable(p_stack, ' ')));
                END;
                $$ LANGUAGE plpgsql IMMUTABLE;
            """);

            migrationBuilder.AlterColumn<string>(
                name: "Apelido",
                table: "Persons",
                type: "character varying(32)",
                maxLength: 32,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(32)",
                oldMaxLength: 32);

            migrationBuilder.AlterColumn<string>(
                  name: "SearchField",
                  table: "Persons",
                  type: "text",
                  nullable: true,
                  computedColumnSql: "generate_search_field(\"Apelido\", \"Nome\", \"Nascimento\", \"Stack\")",
                  stored: true,
                  oldClrType: typeof(string),
                  oldType: "text",
                  oldComputedColumnSql: " unaccent(lower(\"Apelido\")) || ' ' || \r\n unaccent(lower(\"Nome\")) || ' ' || \r\n unaccent(lower(array_to_string_immutable(\"Stack\", ' ')))",
                  oldStored: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP FUNCTION IF EXISTS generate_search_field(varchar, varchar, date, text[]);");
            migrationBuilder.AlterColumn<string>(
                name: "Apelido",
                table: "Persons",
                type: "character varying(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(32)",
                oldMaxLength: 32,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                  name: "SearchField",
                  table: "Persons",
                  type: "text",
                  nullable: false,
                  computedColumnSql: " unaccent(lower(\"Apelido\")) || ' ' || \r\n unaccent(lower(\"Nome\")) || ' ' || \r\n unaccent(lower(array_to_string_immutable(\"Stack\", ' ')))",
                  stored: true,
                  oldClrType: typeof(string),
                  oldType: "text",
                  oldNullable: true,
                  oldComputedColumnSql: "generate_search_field(\"Apelido\", \"Nome\", \"Nascimento\", \"Stack\")",
                  oldStored: true);

        }
    }
}
