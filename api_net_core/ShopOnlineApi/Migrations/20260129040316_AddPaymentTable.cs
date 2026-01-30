using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopOnline.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PayPalOrderId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PayPalCaptureId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2026, 1, 29, 4, 3, 15, 651, DateTimeKind.Utc).AddTicks(9943), new DateTime(2026, 1, 29, 4, 3, 15, 651, DateTimeKind.Utc).AddTicks(9943) });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2026, 1, 29, 4, 3, 15, 651, DateTimeKind.Utc).AddTicks(9945), new DateTime(2026, 1, 29, 4, 3, 15, 651, DateTimeKind.Utc).AddTicks(9946) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2026, 1, 29, 4, 3, 15, 651, DateTimeKind.Utc).AddTicks(9922), new DateTime(2026, 1, 29, 4, 3, 15, 651, DateTimeKind.Utc).AddTicks(9922) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2026, 1, 29, 4, 3, 15, 651, DateTimeKind.Utc).AddTicks(9925), new DateTime(2026, 1, 29, 4, 3, 15, 651, DateTimeKind.Utc).AddTicks(9925) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2026, 1, 6, 2, 17, 44, 899, DateTimeKind.Utc).AddTicks(5408), new DateTime(2026, 1, 6, 2, 17, 44, 899, DateTimeKind.Utc).AddTicks(5409) });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2026, 1, 6, 2, 17, 44, 899, DateTimeKind.Utc).AddTicks(5411), new DateTime(2026, 1, 6, 2, 17, 44, 899, DateTimeKind.Utc).AddTicks(5412) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2026, 1, 6, 2, 17, 44, 899, DateTimeKind.Utc).AddTicks(5385), new DateTime(2026, 1, 6, 2, 17, 44, 899, DateTimeKind.Utc).AddTicks(5385) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2026, 1, 6, 2, 17, 44, 899, DateTimeKind.Utc).AddTicks(5388), new DateTime(2026, 1, 6, 2, 17, 44, 899, DateTimeKind.Utc).AddTicks(5389) });
        }
    }
}
