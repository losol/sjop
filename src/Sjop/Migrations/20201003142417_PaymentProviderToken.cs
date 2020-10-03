using Microsoft.EntityFrameworkCore.Migrations;

namespace Sjop.Migrations
{
    public partial class PaymentProviderToken : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PaymentProviderToken",
                table: "Orders",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentProviderToken",
                table: "Orders");
        }
    }
}
