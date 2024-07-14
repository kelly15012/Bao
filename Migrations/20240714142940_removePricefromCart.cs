using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bao.Migrations
{
    /// <inheritdoc />
    public partial class removePricefromCart : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "Carts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Carts",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
