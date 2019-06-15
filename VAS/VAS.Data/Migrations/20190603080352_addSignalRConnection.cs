using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VAS.Data.Migrations
{
    public partial class addSignalRConnection : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "signalRConnections",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ConnectionId = table.Column<string>(nullable: true),
                    Username = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_signalRConnections", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "signalRConnections");
        }
    }
}
