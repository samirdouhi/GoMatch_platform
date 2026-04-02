using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProfileService.Migrations
{
    /// <inheritdoc />
    public partial class ReplacePhotoUrlWithPhotoPath : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PhotoUrl",
                table: "Profiles",
                newName: "PhotoPath");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PhotoPath",
                table: "Profiles",
                newName: "PhotoUrl");
        }
    }
}
