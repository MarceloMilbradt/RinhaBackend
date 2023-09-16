using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RinhaBackend.Migrations
{
    /// <inheritdoc />
    public partial class InitialRinhaBackendMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.Sql("""
                CREATE FUNCTION array_to_string_immutable(
                    arg text[], 
                    separator text,
                    null_string text DEFAULT NULL
                ) RETURNS text IMMUTABLE PARALLEL SAFE LANGUAGE SQL AS $$
                    SELECT string_agg(
                        CASE WHEN element IS NULL THEN null_string ELSE element END,
                        separator
                    )
                    FROM unnest(arg) AS element;
                $$;
                """);

            migrationBuilder.CreateTable(
                name: "Persons",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Apelido = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Nascimento = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Stack = table.Column<List<string>>(type: "text[]", nullable: false),
                    SearchField = table.Column<string>(type: "text", nullable: false, computedColumnSql: "\"Apelido\" || ' ' || \"Nome\" || ' ' || array_to_string_immutable(\"Stack\", ' ')", stored: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Persons", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Persons_Id",
                table: "Persons",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Persons_SearchField",
                table: "Persons",
                column: "SearchField");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Persons");
            migrationBuilder.Sql("DROP FUNCTION array_to_string_immutable(text[], text, text);");
        }
    }
}
