using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GreenFieldLocalHub.Data.Migrations
{
    /// <inheritdoc />
    public partial class ImageFarmer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "Farmers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "Farmers");
        }
    }
}
