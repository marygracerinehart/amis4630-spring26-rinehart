using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BuckeyeMarketplaceAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddStockQuantityToProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Carts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Carts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    Category = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SellerName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    PostedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ImageUrl = table.Column<string>(type: "TEXT", nullable: true),
                    StockQuantity = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CartItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CartId = table.Column<int>(type: "INTEGER", nullable: false),
                    ProductId = table.Column<int>(type: "INTEGER", nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ImageUrl = table.Column<string>(type: "TEXT", nullable: true),
                    Category = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    SellerName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CartItems_Carts_CartId",
                        column: x => x.CartId,
                        principalTable: "Carts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CartItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Category", "Description", "ImageUrl", "PostedDate", "Price", "SellerName", "StockQuantity", "Title" },
                values: new object[,]
                {
                    { 1, "Textbooks", "Comprehensive textbook for IS fundamentals", "https://m.media-amazon.com/images/I/61wDhC8nNGL._AC_UF1000,1000_QL80_.jpg", new DateTime(2026, 2, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 89.99m, "John Smith", 10, "Introduction to Information Systems textbook" },
                    { 2, "Apparel", "Official Ohio State University sweatshirt", "", new DateTime(2026, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 34.99m, "Emma Johnson", 10, "Ohio State Sweatshirt - Medium" },
                    { 3, "Textbooks", "Advanced organic chemistry reference", "", new DateTime(2026, 2, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 129.99m, "Alex Martinez", 10, "Organic Chemistry Textbook" },
                    { 4, "Electronics", "LED desk lamp with adjustable brightness", "https://encrypted-tbn2.gstatic.com/shopping?q=tbn:ANd9GcRQDvUa6IAUeyY01b9MIIP3a8rZcNZQq-kVNR0ylUQpERW9KsT7UtDHQD5Hab91d8Q-JZGCopcfrnPiXptjLaRR2qXx1KHXM5RnAbyR-nYfEB7r_4Hyhxt0rA", new DateTime(2026, 2, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), 24.99m, "Sarah Lee", 10, "Desk Lamp" },
                    { 5, "Apparel", "Warm winter coat with waterproof exterior", "", new DateTime(2026, 2, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 99.99m, "Michael Brown", 10, "Winter Coat" },
                    { 6, "Electronics", "55-inch 4K Smart TV", "", new DateTime(2026, 2, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), 399.99m, "David Wilson", 10, "TV" },
                    { 7, "Appliances", "Compact mini fridge for dorm or office", "", new DateTime(2026, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 79.99m, "Jessica Garcia", 10, "Mini Refrigerator" },
                    { 8, "Apparel", "Official Ohio State University jacket", "", new DateTime(2026, 2, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), 64.99m, "Christopher Davis", 10, "Ohio State Jacket" },
                    { 9, "Home Decor", "Large wall mirror with wooden frame", "", new DateTime(2026, 3, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), 44.99m, "Amanda Rodriguez", 10, "Mirror" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_CartId",
                table: "CartItems",
                column: "CartId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_ProductId",
                table: "CartItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Carts_UserId",
                table: "Carts",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CartItems");

            migrationBuilder.DropTable(
                name: "Carts");

            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
