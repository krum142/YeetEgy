using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Yeetegy.Data.Migrations
{
    public partial class AddSelfReferencingCommentReplays : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserReplayVotes");

            migrationBuilder.DropTable(
                name: "Replays");

            migrationBuilder.AddColumn<string>(
                name: "ReplayId",
                table: "Comments",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Comments_ReplayId",
                table: "Comments",
                column: "ReplayId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Comments_ReplayId",
                table: "Comments",
                column: "ReplayId",
                principalTable: "Comments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Comments_ReplayId",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Comments_ReplayId",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "ReplayId",
                table: "Comments");

            migrationBuilder.CreateTable(
                name: "Replays",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CommentId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(2500)", maxLength: 2500, nullable: false),
                    Dislikes = table.Column<int>(type: "int", nullable: false),
                    ImgUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Likes = table.Column<int>(type: "int", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Replays", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Replays_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Replays_Comments_CommentId",
                        column: x => x.CommentId,
                        principalTable: "Comments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserReplayVotes",
                columns: table => new
                {
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ReplayId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Id = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserReplayVotes", x => new { x.ApplicationUserId, x.ReplayId });
                    table.ForeignKey(
                        name: "FK_UserReplayVotes_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserReplayVotes_Replays_ReplayId",
                        column: x => x.ReplayId,
                        principalTable: "Replays",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Replays_ApplicationUserId",
                table: "Replays",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Replays_CommentId",
                table: "Replays",
                column: "CommentId");

            migrationBuilder.CreateIndex(
                name: "IX_Replays_IsDeleted",
                table: "Replays",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_UserReplayVotes_IsDeleted",
                table: "UserReplayVotes",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_UserReplayVotes_ReplayId",
                table: "UserReplayVotes",
                column: "ReplayId");
        }
    }
}
