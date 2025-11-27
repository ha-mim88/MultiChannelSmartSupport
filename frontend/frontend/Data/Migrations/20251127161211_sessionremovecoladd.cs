using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace frontend.Migrations
{
    /// <inheritdoc />
    public partial class sessionremovecoladd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Removed",
                table: "AISession",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Removed",
                table: "AISession");
        }
    }
}
