using Microsoft.EntityFrameworkCore.Migrations;

namespace Sjop.Migrations
{
    public partial class PaymentId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PaymentProviderSessionId",
                table: "Orders",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentProviderSessionId",
                table: "Orders");
        }
    }
}
