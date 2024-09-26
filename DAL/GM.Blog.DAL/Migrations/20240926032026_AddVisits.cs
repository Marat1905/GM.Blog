using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GM.Blog.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddVisits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tags_Users_UserId",
                table: "Tags");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Posts_PostId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_PostId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Tags_UserId",
                table: "Tags");

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("04ac163c-0aef-43e9-9b09-9bd586f17caf"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("bf49ee80-6ebb-4a8f-bb85-a9f0815bb532"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("e6ad4b3a-d065-492d-b549-4f53aaaf8803"));

            migrationBuilder.DropColumn(
                name: "PostId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Tags");

            migrationBuilder.CreateTable(
                name: "UserToVisitedPost",
                columns: table => new
                {
                    UsersId = table.Column<Guid>(type: "TEXT", nullable: false),
                    VisitedPostsId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserToVisitedPost", x => new { x.UsersId, x.VisitedPostsId });
                    table.ForeignKey(
                        name: "FK_UserToVisitedPost_Posts_VisitedPostsId",
                        column: x => x.VisitedPostsId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserToVisitedPost_Users_UsersId",
                        column: x => x.UsersId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "ConcurrencyStamp", "Description", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("71f1d934-009e-4995-8e26-ba7c2b909da6"), null, "Роль с максимальными возможностями в приложении", "Admin", "ADMIN" },
                    { new Guid("ab7d8774-343c-4d45-bed5-006ad0eb5b8b"), null, "Стандартная роль в приложении", "User", "USER" },
                    { new Guid("f020cd3e-249d-40ff-b229-ad1d9af36ec7"), null, "Данная роль позволяет выполнять редактирование, удаление комментариев и статей в приложении", "Moderator", "MODERATOR" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserToVisitedPost_VisitedPostsId",
                table: "UserToVisitedPost",
                column: "VisitedPostsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserToVisitedPost");

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("71f1d934-009e-4995-8e26-ba7c2b909da6"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("ab7d8774-343c-4d45-bed5-006ad0eb5b8b"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("f020cd3e-249d-40ff-b229-ad1d9af36ec7"));

            migrationBuilder.AddColumn<Guid>(
                name: "PostId",
                table: "Users",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Tags",
                type: "TEXT",
                nullable: true);

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "ConcurrencyStamp", "Description", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("04ac163c-0aef-43e9-9b09-9bd586f17caf"), null, "Данная роль позволяет выполнять редактирование, удаление комментариев и статей в приложении", "Moderator", "MODERATOR" },
                    { new Guid("bf49ee80-6ebb-4a8f-bb85-a9f0815bb532"), null, "Стандартная роль в приложении", "User", "USER" },
                    { new Guid("e6ad4b3a-d065-492d-b549-4f53aaaf8803"), null, "Роль с максимальными возможностями в приложении", "Admin", "ADMIN" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_PostId",
                table: "Users",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_UserId",
                table: "Tags",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_Users_UserId",
                table: "Tags",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Posts_PostId",
                table: "Users",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id");
        }
    }
}
