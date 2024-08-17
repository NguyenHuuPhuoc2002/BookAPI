using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTableNXB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TenNhaXuaBan",
                table: "NhaXuatBan",
                newName: "TenNhaXuatBan");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TenNhaXuatBan",
                table: "NhaXuatBan",
                newName: "TenNhaXuaBan");
        }
    }
}
