using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kursova_VideoStore.Migrations
{
    /// <inheritdoc />
    public partial class AddDescriptionAndSoftDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Films",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Films",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Films");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Films");
        }
    }
}
