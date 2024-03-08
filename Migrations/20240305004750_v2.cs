using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GraduationProject.Migrations
{
    /// <inheritdoc />
    public partial class v2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DiscountHourRate",
                table: "generalSettings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ExtraHourRate",
                table: "generalSettings",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscountHourRate",
                table: "generalSettings");

            migrationBuilder.DropColumn(
                name: "ExtraHourRate",
                table: "generalSettings");
        }
    }
}
