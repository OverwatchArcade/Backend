using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OWArcadeBackend.Persistence.Migrations
{
    public partial class ArcadeModeKeys : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TileMode_ArcadeModes_ArcadeModeId",
                table: "TileMode");

            migrationBuilder.AlterColumn<string>(
                name: "Players",
                table: "ArcadeModes",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ArcadeModes",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.UpdateData(
                table: "Contributors",
                keyColumn: "Id",
                keyValue: new Guid("e992ded4-30ca-4cdd-9047-d7f0a5ab6378"),
                column: "RegisteredAt",
                value: new DateTime(2022, 1, 27, 20, 37, 37, 172, DateTimeKind.Utc).AddTicks(9316));

            migrationBuilder.CreateIndex(
                name: "IX_ArcadeModes_Game_Name_Players",
                table: "ArcadeModes",
                columns: new[] { "Game", "Name", "Players" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TileMode_ArcadeModes_ArcadeModeId",
                table: "TileMode",
                column: "ArcadeModeId",
                principalTable: "ArcadeModes",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TileMode_ArcadeModes_ArcadeModeId",
                table: "TileMode");

            migrationBuilder.DropIndex(
                name: "IX_ArcadeModes_Game_Name_Players",
                table: "ArcadeModes");

            migrationBuilder.AlterColumn<string>(
                name: "Players",
                table: "ArcadeModes",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ArcadeModes",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.UpdateData(
                table: "Contributors",
                keyColumn: "Id",
                keyValue: new Guid("e992ded4-30ca-4cdd-9047-d7f0a5ab6378"),
                column: "RegisteredAt",
                value: new DateTime(2021, 9, 29, 17, 53, 11, 876, DateTimeKind.Utc).AddTicks(1140));

            migrationBuilder.AddForeignKey(
                name: "FK_TileMode_ArcadeModes_ArcadeModeId",
                table: "TileMode",
                column: "ArcadeModeId",
                principalTable: "ArcadeModes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
