using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WhishList.Migrations
{
    /// <inheritdoc />
    public partial class AddPriceAndCurrencyToWish : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "PriceAmount",
                table: "Wishes",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "PriceCurrencyCode",
                table: "Wishes",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PriceAmount",
                table: "Wishes");

            migrationBuilder.DropColumn(
                name: "PriceCurrencyCode",
                table: "Wishes");
        }
    }
}
