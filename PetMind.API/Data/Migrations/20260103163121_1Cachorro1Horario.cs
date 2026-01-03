using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetMind.API.Migrations
{
    /// <inheritdoc />
    public partial class _1Cachorro1Horario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CachorroIds",
                table: "Horarios");

            migrationBuilder.AddColumn<int>(
                name: "CachorroId",
                table: "Horarios",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CachorroId",
                table: "Horarios");

            migrationBuilder.AddColumn<string>(
                name: "CachorroIds",
                table: "Horarios",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
