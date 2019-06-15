using Microsoft.EntityFrameworkCore.Migrations;

namespace VAS.Data.Migrations
{
    public partial class huy30052019 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SpecialityId",
                table: "Schedulings",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SpecialityId",
                table: "Schedulings");
        }
    }
}
