using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Korvan_API.Migrations
{
    /// <inheritdoc />
    public partial class Avatars : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AvatarUrl",
                table: "Users",
                newName: "AvatarContentType");

            migrationBuilder.AddColumn<byte[]>(
                name: "AvatarImage",
                table: "Users",
                type: "bytea",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvatarImage",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "AvatarContentType",
                table: "Users",
                newName: "AvatarUrl");
        }
    }
}
