using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ShopOnline.Api.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "CreatedDate", "Description", "Name", "UpdatedDate" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 6, 9, 1, 11, 0, 117, DateTimeKind.Utc).AddTicks(6238), "Electronics", "Electronics", new DateTime(2026, 6, 9, 1, 11, 0, 117, DateTimeKind.Utc).AddTicks(6715) },
                    { 2, new DateTime(2026, 6, 9, 1, 11, 0, 117, DateTimeKind.Utc).AddTicks(7179), "Accessories", "Accessories", new DateTime(2026, 6, 9, 1, 11, 0, 117, DateTimeKind.Utc).AddTicks(7179) }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Description", "Name", "Price", "Quantity" },
                values: new object[,]
                {
                    { 1, "Apple flagship", "iPhone 15", 999m, 10 },
                    { 2, "Samsung flagship", "Samsung S24", 899m, 15 }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedDate", "Email", "FirstName", "LastName", "UpdatedDate" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 6, 9, 1, 11, 0, 117, DateTimeKind.Utc).AddTicks(2562), "emily.johnson@x.dummyjson.com", "Emily", "Johnson", new DateTime(2026, 6, 9, 1, 11, 0, 117, DateTimeKind.Utc).AddTicks(3267) },
                    { 2, new DateTime(2026, 6, 9, 1, 11, 0, 117, DateTimeKind.Utc).AddTicks(3740), "michael.williams@x.dummyjson.com", "Michael", "Williams", new DateTime(2026, 6, 9, 1, 11, 0, 117, DateTimeKind.Utc).AddTicks(3740) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
