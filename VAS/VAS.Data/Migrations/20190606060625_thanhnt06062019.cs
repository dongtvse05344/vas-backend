﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace VAS.Data.Migrations
{
    public partial class thanhnt06062019 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "status",
                table: "Tickets",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "status",
                table: "Tickets");
        }
    }
}
