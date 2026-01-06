using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetMind.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PetShops",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Senha = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EnderecoPetShop = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PetShops", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Cachorros",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomeCachorro = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NomeTutor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContatoTutor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EnderecoCachorro = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Raca = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Porte = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PetShopId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cachorros", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cachorros_PetShops_PetShopId",
                        column: x => x.PetShopId,
                        principalTable: "PetShops",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Horarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CachorroId = table.Column<int>(type: "int", nullable: false),
                    PetShopId = table.Column<int>(type: "int", nullable: false),
                    Data = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ValorTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ServicoBaseSelecionado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Adicionais = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Horarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Horarios_Cachorros_CachorroId",
                        column: x => x.CachorroId,
                        principalTable: "Cachorros",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Horarios_PetShops_PetShopId",
                        column: x => x.PetShopId,
                        principalTable: "PetShops",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cachorros_PetShopId",
                table: "Cachorros",
                column: "PetShopId");

            migrationBuilder.CreateIndex(
                name: "IX_Horarios_CachorroId",
                table: "Horarios",
                column: "CachorroId");

            migrationBuilder.CreateIndex(
                name: "IX_Horarios_PetShopId",
                table: "Horarios",
                column: "PetShopId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Horarios");

            migrationBuilder.DropTable(
                name: "Cachorros");

            migrationBuilder.DropTable(
                name: "PetShops");
        }
    }
}
