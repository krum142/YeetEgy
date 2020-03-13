using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Yeetegy.Data.Migrations
{
    public partial class UserLikePostbaseDeletableAdd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "UserLikePosts",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedOn",
                table: "UserLikePosts",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Id",
                table: "UserLikePosts",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "UserLikePosts",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedOn",
                table: "UserLikePosts",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserLikePosts_IsDeleted",
                table: "UserLikePosts",
                column: "IsDeleted");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserLikePosts_IsDeleted",
                table: "UserLikePosts");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "UserLikePosts");

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "UserLikePosts");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "UserLikePosts");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "UserLikePosts");

            migrationBuilder.DropColumn(
                name: "ModifiedOn",
                table: "UserLikePosts");
        }
    }
}
