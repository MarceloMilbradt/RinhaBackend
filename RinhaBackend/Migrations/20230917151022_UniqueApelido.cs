using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RinhaBackend.Migrations
{
    /// <inheritdoc />
    public partial class UniqueApelido : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "SearchField",
                table: "Persons",
                type: "text",
                nullable: true,
                computedColumnSql: "generate_search_field(\"Apelido\", \"Nome\", \"Nascimento\", \"Stack\")",
                stored: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true,
                oldComputedColumnSql: " immutable_unaccent(lower(\"Apelido\")) || ' ' || \r\n immutable_unaccent(lower(\"Nome\")) || ' ' || \r\n CURRENT_DATE || ' ' || \r\n immutable_unaccent(lower(array_to_string_immutable(\"Stack\", ' ')))",
                oldStored: true);

            migrationBuilder.CreateIndex(
                name: "IX_Persons_Apelido",
                table: "Persons",
                column: "Apelido",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Persons_Apelido",
                table: "Persons");

            migrationBuilder.AlterColumn<string>(
                name: "SearchField",
                table: "Persons",
                type: "text",
                nullable: true,
                computedColumnSql: " immutable_unaccent(lower(\"Apelido\")) || ' ' || \r\n immutable_unaccent(lower(\"Nome\")) || ' ' || \r\n CURRENT_DATE || ' ' || \r\n immutable_unaccent(lower(array_to_string_immutable(\"Stack\", ' ')))",
                stored: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true,
                oldComputedColumnSql: "generate_search_field(\"Apelido\", \"Nome\", \"Nascimento\", \"Stack\")",
                oldStored: true);
        }
    }
}
