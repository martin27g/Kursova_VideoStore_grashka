using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kursova_VideoStore.Migrations
{
    /// <inheritdoc />
    public partial class AddRentalDates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DueDate",
                table: "OrderDetails",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ReturnDate",
                table: "OrderDetails",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DueDate",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "ReturnDate",
                table: "OrderDetails");
        }
    }
}
