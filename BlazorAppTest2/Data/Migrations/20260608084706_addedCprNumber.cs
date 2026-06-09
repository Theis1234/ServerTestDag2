using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorAppTest2.Migrations
{
    /// <inheritdoc />
    public partial class addedCprNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CprNumber",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CprNumber",
                table: "AspNetUsers");
        }
    }
}
