using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VAS.Data.Migrations
{
    public partial class thanhnt30052019 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SpecialityId",
                table: "Schedulings");

            migrationBuilder.AddColumn<string>(
                name: "SpecialityName",
                table: "Schedulings",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TotalTicket",
                table: "Schedulings",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SpecialityName",
                table: "Schedulings");

            migrationBuilder.DropColumn(
                name: "TotalTicket",
                table: "Schedulings");

            migrationBuilder.AddColumn<Guid>(
                name: "SpecialityId",
                table: "Schedulings",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
