using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OverwatchArcade.Persistence.Migrations
{
    public partial class RemovedGameType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ArcadeModes_Game_Name_Players",
                table: "ArcadeModes");
            
            migrationBuilder.DeleteData(
                table: "Config",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Dailies",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DropColumn(
                name: "Game",
                table: "Dailies");

            migrationBuilder.DropColumn(
                name: "Game",
                table: "ArcadeModes");

            migrationBuilder.UpdateData(
                table: "Config",
                keyColumn: "Id",
                keyValue: 1,
                column: "Key",
                value: "Countries");

            migrationBuilder.UpdateData(
                table: "Config",
                keyColumn: "Id",
                keyValue: 2,
                column: "Key",
                value: "V1ContributionCount");

            migrationBuilder.UpdateData(
                table: "Config",
                keyColumn: "Id",
                keyValue: 3,
                column: "Key",
                value: "OwTiles");

            migrationBuilder.UpdateData(
                table: "Config",
                keyColumn: "Id",
                keyValue: 4,
                column: "Key",
                value: "OwCurrentEvent");

            migrationBuilder.UpdateData(
                table: "Config",
                keyColumn: "Id",
                keyValue: 5,
                column: "Key",
                value: "OwMaps");

            migrationBuilder.UpdateData(
                table: "Config",
                keyColumn: "Id",
                keyValue: 6,
                column: "Key",
                value: "OwHeroes");

            migrationBuilder.CreateIndex(
                name: "IX_ArcadeModes_Name_Players",
                table: "ArcadeModes",
                columns: new[] { "Name", "Players" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ArcadeModes_Name_Players",
                table: "ArcadeModes");

            migrationBuilder.AddColumn<int>(
                name: "Game",
                table: "Dailies",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Game",
                table: "ArcadeModes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Config",
                keyColumn: "Id",
                keyValue: 1,
                column: "Key",
                value: "COUNTRIES");

            migrationBuilder.UpdateData(
                table: "Config",
                keyColumn: "Id",
                keyValue: 2,
                column: "Key",
                value: "V1_CONTRIBUTION_COUNT");

            migrationBuilder.UpdateData(
                table: "Config",
                keyColumn: "Id",
                keyValue: 3,
                column: "Key",
                value: "OW_TILES");

            migrationBuilder.UpdateData(
                table: "Config",
                keyColumn: "Id",
                keyValue: 4,
                column: "Key",
                value: "OW_CURRENT_EVENT");

            migrationBuilder.UpdateData(
                table: "Config",
                keyColumn: "Id",
                keyValue: 5,
                column: "Key",
                value: "OW_MAPS");

            migrationBuilder.UpdateData(
                table: "Config",
                keyColumn: "Id",
                keyValue: 6,
                column: "Key",
                value: "OW_HEROES");

            migrationBuilder.InsertData(
                table: "Config",
                columns: new[] { "Id", "JsonValue", "Key", "Value" },
                values: new object[] { 7, null, "OW2_TILES", "7" });

            migrationBuilder.InsertData(
                table: "Dailies",
                columns: new[] { "Id", "ContributorId", "CreatedAt", "Game", "MarkedOverwrite" },
                values: new object[] { 2, new Guid("e992ded4-30ca-4cdd-9047-d7f0a5ab6378"), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, false });

            migrationBuilder.CreateIndex(
                name: "IX_ArcadeModes_Game_Name_Players",
                table: "ArcadeModes",
                columns: new[] { "Game", "Name", "Players" },
                unique: true);
        }
    }
}
