using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication2.Data.Migrations
{
    public partial class Post4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "date",
                table: "Posts",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "link",
                table: "Posts",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ownersName",
                table: "Posts",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "text",
                table: "Posts",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Comment",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    PostId = table.Column<Guid>(nullable: true),
                    ownersName = table.Column<string>(nullable: true),
                    text = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comment_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comment_PostId",
                table: "Comment",
                column: "PostId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Comment");

            migrationBuilder.DropColumn(
                name: "date",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "link",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "ownersName",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "text",
                table: "Posts");
        }
    }
}
