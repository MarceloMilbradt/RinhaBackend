using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RinhaBackend.Migrations
{
    /// <inheritdoc />
    public partial class ChangeNascimentoToDateType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateOnly>(
                name: "Nascimento",
                table: "Persons",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "SearchField",
                table: "Persons",
                type: "text",
                nullable: false,
                computedColumnSql: "\"Apelido\" || ' ' || \"Nome\" || ' ' || array_to_string_immutable(\"Stack\", ' ')",
                stored: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldComputedColumnSql: "Apelido || ' ' || Nome || ' ' || array_to_string_immutable(Stack, ' ')",
                oldStored: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "Nascimento",
                table: "Persons",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AlterColumn<string>(
                name: "SearchField",
                table: "Persons",
                type: "text",
                nullable: false,
                computedColumnSql: "Apelido || ' ' || Nome || ' ' || array_to_string_immutable(Stack, ' ')",
                stored: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldComputedColumnSql: "\"Apelido\" || ' ' || \"Nome\" || ' ' || array_to_string_immutable(\"Stack\", ' ')",
                oldStored: true);
        }
    }
}
