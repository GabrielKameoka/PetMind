using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetMind.API.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cachorros_Horarios_HorarioId",
                table: "Cachorros");

            migrationBuilder.DropForeignKey(
                name: "FK_Horarios_PetShops_PetShopId",
                table: "Horarios");

            migrationBuilder.DropIndex(
                name: "IX_Cachorros_HorarioId",
                table: "Cachorros");

            migrationBuilder.DropColumn(
                name: "HorarioId",
                table: "Cachorros");

            migrationBuilder.AlterColumn<int>(
                name: "PetShopId",
                table: "Horarios",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "HorarioCachorros",
                columns: table => new
                {
                    CachorrosId = table.Column<int>(type: "int", nullable: false),
                    HorarioId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HorarioCachorros", x => new { x.CachorrosId, x.HorarioId });
                    table.ForeignKey(
                        name: "FK_HorarioCachorros_Cachorros_CachorrosId",
                        column: x => x.CachorrosId,
                        principalTable: "Cachorros",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HorarioCachorros_Horarios_HorarioId",
                        column: x => x.HorarioId,
                        principalTable: "Horarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HorarioCachorros_HorarioId",
                table: "HorarioCachorros",
                column: "HorarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Horarios_PetShops_PetShopId",
                table: "Horarios",
                column: "PetShopId",
                principalTable: "PetShops",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Horarios_PetShops_PetShopId",
                table: "Horarios");

            migrationBuilder.DropTable(
                name: "HorarioCachorros");

            migrationBuilder.AlterColumn<int>(
                name: "PetShopId",
                table: "Horarios",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "HorarioId",
                table: "Cachorros",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cachorros_HorarioId",
                table: "Cachorros",
                column: "HorarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cachorros_Horarios_HorarioId",
                table: "Cachorros",
                column: "HorarioId",
                principalTable: "Horarios",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Horarios_PetShops_PetShopId",
                table: "Horarios",
                column: "PetShopId",
                principalTable: "PetShops",
                principalColumn: "Id");
        }
    }
}
