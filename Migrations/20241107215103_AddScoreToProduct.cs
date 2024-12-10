using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProductCategoryCrud.Migrations
{
    /// <inheritdoc />
    public partial class AddScoreToProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Score",
                table: "Products",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Score",
                table: "Products");
        }
    }
}
