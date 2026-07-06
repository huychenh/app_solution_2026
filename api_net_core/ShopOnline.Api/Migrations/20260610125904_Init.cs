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
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActived = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
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
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActived = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
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
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActived = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "CreatedBy", "CreatedDate", "Description", "IsActived", "IsDeleted", "Name", "UpdatedBy", "UpdatedDate" },
                values: new object[,]
                {
                    { 1, "System", new DateTime(2026, 6, 10, 12, 59, 2, 405, DateTimeKind.Utc).AddTicks(9670), "Electronics", true, false, "Electronics", "System", new DateTime(2026, 6, 10, 12, 59, 2, 406, DateTimeKind.Utc).AddTicks(142) },
                    { 2, "System", new DateTime(2026, 6, 10, 12, 59, 2, 406, DateTimeKind.Utc).AddTicks(2403), "Accessories", true, false, "Accessories", "System", new DateTime(2026, 6, 10, 12, 59, 2, 406, DateTimeKind.Utc).AddTicks(2404) }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CreatedBy", "CreatedDate", "Description", "IsActived", "IsDeleted", "Name", "Price", "Quantity", "UpdatedBy", "UpdatedDate" },
                values: new object[,]
                {
                    { 1, "System", new DateTime(2026, 6, 10, 12, 59, 2, 404, DateTimeKind.Utc).AddTicks(682), "Apple flagship", true, false, "iPhone 15", 999m, 10, "System", new DateTime(2026, 6, 10, 12, 59, 2, 404, DateTimeKind.Utc).AddTicks(1155) },
                    { 2, "System", new DateTime(2026, 6, 10, 12, 59, 2, 404, DateTimeKind.Utc).AddTicks(3534), "Samsung flagship", true, false, "Samsung S24", 899m, 15, "System", new DateTime(2026, 6, 10, 12, 59, 2, 404, DateTimeKind.Utc).AddTicks(3535) }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedBy", "CreatedDate", "Email", "FirstName", "IsActived", "IsDeleted", "LastName", "UpdatedBy", "UpdatedDate" },
                values: new object[,]
                {
                    { 1, "System", new DateTime(2026, 6, 10, 12, 59, 2, 405, DateTimeKind.Utc).AddTicks(4306), "emily.johnson@x.dummyjson.com", "Emily", true, false, "Johnson", "System", new DateTime(2026, 6, 10, 12, 59, 2, 405, DateTimeKind.Utc).AddTicks(4788) },
                    { 2, "System", new DateTime(2026, 6, 10, 12, 59, 2, 405, DateTimeKind.Utc).AddTicks(7050), "michael.williams@x.dummyjson.com", "Michael", true, false, "Williams", "System", new DateTime(2026, 6, 10, 12, 59, 2, 405, DateTimeKind.Utc).AddTicks(7051) }
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
