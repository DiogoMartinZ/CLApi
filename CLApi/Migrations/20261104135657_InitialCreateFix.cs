using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CLApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreateFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 100001,
                column: "DateCreated",
                value: new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 100002,
                column: "DateCreated",
                value: new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 100001,
                column: "DateCreated",
                value: new DateTime(2026, 11, 2, 10, 50, 35, 54, DateTimeKind.Utc).AddTicks(8391));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 100002,
                column: "DateCreated",
                value: new DateTime(2026, 11, 2, 10, 50, 35, 54, DateTimeKind.Utc).AddTicks(8897));
        }
    }
}
