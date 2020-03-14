using Microsoft.EntityFrameworkCore.Migrations;

namespace Yeetegy.Data.Migrations
{
    public partial class AddUserPostVoteValue : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Dislike",
                table: "UserPostVotes");

            migrationBuilder.DropColumn(
                name: "Like",
                table: "UserPostVotes");

            migrationBuilder.AddColumn<string>(
                name: "Value",
                table: "UserPostVotes",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Value",
                table: "UserPostVotes");

            migrationBuilder.AddColumn<bool>(
                name: "Dislike",
                table: "UserPostVotes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Like",
                table: "UserPostVotes",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
