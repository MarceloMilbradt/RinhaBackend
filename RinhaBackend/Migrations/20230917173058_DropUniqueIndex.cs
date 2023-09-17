using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RinhaBackend.Migrations
{
    /// <inheritdoc />
    public partial class DropUniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Persons_Apelido",
                table: "Persons");

            migrationBuilder.CreateIndex(
                name: "IX_Persons_Apelido",
                table: "Persons",
                column: "Apelido");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Persons_Apelido",
                table: "Persons");

            migrationBuilder.CreateIndex(
                name: "IX_Persons_Apelido",
                table: "Persons",
                column: "Apelido",
                unique: true);
        }
    }
}
