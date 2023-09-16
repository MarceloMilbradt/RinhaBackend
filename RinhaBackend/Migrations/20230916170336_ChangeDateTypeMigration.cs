using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RinhaBackend.Migrations
{
    /// <inheritdoc />
    public partial class ChangeDateTypeMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "SearchField",
                table: "Persons",
                type: "text",
                nullable: false,
                computedColumnSql: "\"Apelido\" || ' ' || \"Nome\" || ' ' || array_to_string_immutable(\"Stack\", ' ')",
                stored: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldComputedColumnSql: "\"Apelido\" || ' ' || \"Nome\" || ' ' || ARRAY_TO_STRING(\"Stack\", ' ')",
                oldStored: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "SearchField",
                table: "Persons",
                type: "text",
                nullable: false,
                computedColumnSql: "\"Apelido\" || ' ' || \"Nome\" || ' ' || ARRAY_TO_STRING(\"Stack\", ' ')",
                stored: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldComputedColumnSql: "\"Apelido\" || ' ' || \"Nome\" || ' ' || array_to_string_immutable(\"Stack\", ' ')",
                oldStored: true);
        }
    }
}
