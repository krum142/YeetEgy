using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Yeetegy.Data.Migrations
{
    public partial class AddUserReplayVoteUserCommentVote : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserCommentVotes",
                columns: table => new
                {
                    CommentId = table.Column<string>(nullable: false),
                    ApplicationUserId = table.Column<string>(nullable: false),
                    Id = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    ModifiedOn = table.Column<DateTime>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DeletedOn = table.Column<DateTime>(nullable: true),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCommentVotes", x => new { x.ApplicationUserId, x.CommentId });
                    table.ForeignKey(
                        name: "FK_UserCommentVotes_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserCommentVotes_Comments_CommentId",
                        column: x => x.CommentId,
                        principalTable: "Comments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserReplayVotes",
                columns: table => new
                {
                    ReplayId = table.Column<string>(nullable: false),
                    ApplicationUserId = table.Column<string>(nullable: false),
                    Id = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    ModifiedOn = table.Column<DateTime>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DeletedOn = table.Column<DateTime>(nullable: true),
                    Value = table.Column<string>(nullable: true)
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
                name: "IX_UserCommentVotes_CommentId",
                table: "UserCommentVotes",
                column: "CommentId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCommentVotes_IsDeleted",
                table: "UserCommentVotes",
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserCommentVotes");

            migrationBuilder.DropTable(
                name: "UserReplayVotes");
        }
    }
}
