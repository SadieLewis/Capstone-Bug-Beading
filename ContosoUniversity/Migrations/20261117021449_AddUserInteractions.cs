using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BugBeading.Migrations
{
    /// <inheritdoc />
    public partial class AddUserInteractions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserInteractions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    InteractionType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    ElementType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    ProductId = table.Column<int>(type: "INTEGER", nullable: true),
                    DurationMilliseconds = table.Column<int>(type: "INTEGER", nullable: false),
                    Page = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    TimestampUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserInteractions", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserInteractions");
        }
    }
}