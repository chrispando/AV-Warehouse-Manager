using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SounDesign_Web_02.Data.Migrations
{
    public partial class Create_Database3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "createdTimeStamp",
                table: "Staging",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "createdTimeStamp",
                table: "Staging");
        }
    }
}
