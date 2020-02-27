using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Yeetegy.Data.Migrations
{
    public partial class CategoryImgToString : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Image",
                table: "Categories");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Categories",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Categories");

            migrationBuilder.AddColumn<byte[]>(
                name: "Image",
                table: "Categories",
                type: "varbinary(max)",
                nullable: true);
        }
    }
}
