using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetMind.API.Migrations
{
    /// <inheritdoc />
    public partial class HorarioConfigurado : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CachorroIds",
                table: "Horarios",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CachorroIds",
                table: "Horarios");
        }
    }
}
