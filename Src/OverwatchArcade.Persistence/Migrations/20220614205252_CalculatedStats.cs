using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OverwatchArcade.Persistence.Migrations
{
    public partial class CalculatedStats : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Config_Key",
                table: "Config");

            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "Contributors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Contributors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Avatar",
                table: "Contributors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "default.jpg",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldDefaultValue: "default.jpg");

            migrationBuilder.AddColumn<string>(
                name: "Stats",
                table: "Contributors",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Key",
                table: "Config",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Image",
                table: "ArcadeModes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Config",
                keyColumn: "Id",
                keyValue: 2,
                column: "JsonValue",
                value: "[{\"Key\":\"e992ded4-30ca-4cdd-9047-d7f0a5ab6378\",\"Value\":0}]");

            migrationBuilder.UpdateData(
                table: "Contributors",
                keyColumn: "Id",
                keyValue: new Guid("e992ded4-30ca-4cdd-9047-d7f0a5ab6378"),
                column: "RegisteredAt",
                value: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_Config_Key",
                table: "Config",
                column: "Key",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Config_Key",
                table: "Config");

            migrationBuilder.DropColumn(
                name: "Stats",
                table: "Contributors");

            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "Contributors",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Contributors",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Avatar",
                table: "Contributors",
                type: "nvarchar(max)",
                nullable: true,
                defaultValue: "default.jpg",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldDefaultValue: "default.jpg");

            migrationBuilder.AlterColumn<string>(
                name: "Key",
                table: "Config",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Image",
                table: "ArcadeModes",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.UpdateData(
                table: "Config",
                keyColumn: "Id",
                keyValue: 2,
                column: "JsonValue",
                value: "[{\"UserId\":\"e992ded4-30ca-4cdd-9047-d7f0a5ab6378\",\"Count\":0,\"Name\":null}]");

            migrationBuilder.UpdateData(
                table: "Contributors",
                keyColumn: "Id",
                keyValue: new Guid("e992ded4-30ca-4cdd-9047-d7f0a5ab6378"),
                column: "RegisteredAt",
                value: new DateTime(2022, 1, 27, 20, 37, 37, 172, DateTimeKind.Utc).AddTicks(9316));

            migrationBuilder.CreateIndex(
                name: "IX_Config_Key",
                table: "Config",
                column: "Key",
                unique: true,
                filter: "[Key] IS NOT NULL");
        }
    }
}
