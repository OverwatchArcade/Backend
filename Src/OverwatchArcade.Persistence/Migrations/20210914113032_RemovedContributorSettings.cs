using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OWArcadeBackend.Persistence.Migrations
{
    public partial class RemovedContributorSettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Settings",
                table: "Contributors");
            
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Settings",
                table: "Contributors",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
