using Microsoft.EntityFrameworkCore.Migrations;

namespace FlapKapVendingMachine.Migrations
{
    public partial class StockOrderQuantity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "StockOrders",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "StockOrders");
        }
    }
}
