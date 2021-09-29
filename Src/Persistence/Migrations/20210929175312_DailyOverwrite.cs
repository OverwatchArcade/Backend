using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OWArcadeBackend.Persistence.Migrations
{
    public partial class DailyOverwrite : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "MarkedOverwrite",
                table: "Dailies",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MarkedOverwrite",
                table: "Dailies");
        }
    }
}
