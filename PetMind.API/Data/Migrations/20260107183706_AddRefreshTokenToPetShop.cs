using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetMind.API.Migrations
{
    /// <inheritdoc />
    public partial class AddRefreshTokenToPetShop : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RefreshToken",
                table: "PetShops",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RefreshTokenExpiryTime",
                table: "PetShops",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RefreshToken",
                table: "PetShops");

            migrationBuilder.DropColumn(
                name: "RefreshTokenExpiryTime",
                table: "PetShops");
        }
    }
}
