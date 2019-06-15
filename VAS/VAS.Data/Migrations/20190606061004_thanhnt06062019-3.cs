using Microsoft.EntityFrameworkCore.Migrations;

namespace VAS.Data.Migrations
{
    public partial class thanhnt060620193 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "status",
                table: "Tickets",
                newName: "Status");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Tickets",
                newName: "status");
        }
    }
}
