using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class Small_Changes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_CartPositions_ProductId",
                table: "CartPositions",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_CartPositions_Products_ProductId",
                table: "CartPositions",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartPositions_Products_ProductId",
                table: "CartPositions");

            migrationBuilder.DropIndex(
                name: "IX_CartPositions_ProductId",
                table: "CartPositions");
        }
    }
}
