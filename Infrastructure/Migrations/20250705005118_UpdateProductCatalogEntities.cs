using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProductCatalogEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StockCount",
                table: "ProductStocks",
                newName: "Quantity");

            migrationBuilder.RenameColumn(
                name: "ImageUrl",
                table: "ProductImages",
                newName: "Url");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "ProductStocks",
                newName: "StockCount");

            migrationBuilder.RenameColumn(
                name: "Url",
                table: "ProductImages",
                newName: "ImageUrl");
        }
    }
}
