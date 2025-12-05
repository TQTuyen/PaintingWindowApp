using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaintingApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Profiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Theme = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false, defaultValue: "System"),
                    DefaultCanvasWidth = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 800),
                    DefaultCanvasHeight = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 600),
                    DefaultStrokeColor = table.Column<string>(type: "TEXT", maxLength: 9, nullable: false, defaultValue: "#000000"),
                    DefaultStrokeThickness = table.Column<double>(type: "REAL", nullable: false, defaultValue: 2.0),
                    DefaultStrokeStyle = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false, defaultValue: "Solid"),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Profiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DrawingBoards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Width = table.Column<int>(type: "INTEGER", nullable: false),
                    Height = table.Column<int>(type: "INTEGER", nullable: false),
                    BackgroundColor = table.Column<string>(type: "TEXT", maxLength: 9, nullable: false, defaultValue: "#FFFFFF"),
                    ProfileId = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastModified = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DrawingBoards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DrawingBoards_Profiles_ProfileId",
                        column: x => x.ProfileId,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TemplateGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 300, nullable: true),
                    ProfileId = table.Column<int>(type: "INTEGER", nullable: false),
                    UsageCount = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TemplateGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TemplateGroups_Profiles_ProfileId",
                        column: x => x.ProfileId,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Shapes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Type = table.Column<string>(type: "TEXT", nullable: false),
                    DrawingBoardId = table.Column<int>(type: "INTEGER", nullable: true),
                    TemplateGroupId = table.Column<int>(type: "INTEGER", nullable: true),
                    StrokeColor = table.Column<string>(type: "TEXT", maxLength: 9, nullable: false),
                    StrokeThickness = table.Column<double>(type: "REAL", nullable: false),
                    StrokeStyle = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false, defaultValue: "Solid"),
                    FillColor = table.Column<string>(type: "TEXT", maxLength: 9, nullable: true),
                    GeometryData = table.Column<string>(type: "TEXT", maxLength: 4000, nullable: false),
                    ZIndex = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shapes", x => x.Id);
                    table.CheckConstraint("CK_Shape_ParentConstraint", "([DrawingBoardId] IS NOT NULL AND [TemplateGroupId] IS NULL) OR ([DrawingBoardId] IS NULL AND [TemplateGroupId] IS NOT NULL)");
                    table.ForeignKey(
                        name: "FK_Shapes_DrawingBoards_DrawingBoardId",
                        column: x => x.DrawingBoardId,
                        principalTable: "DrawingBoards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Shapes_TemplateGroups_TemplateGroupId",
                        column: x => x.TemplateGroupId,
                        principalTable: "TemplateGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DrawingBoards_ProfileId",
                table: "DrawingBoards",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Profiles_Name",
                table: "Profiles",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Shapes_DrawingBoardId",
                table: "Shapes",
                column: "DrawingBoardId");

            migrationBuilder.CreateIndex(
                name: "IX_Shapes_TemplateGroupId",
                table: "Shapes",
                column: "TemplateGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_TemplateGroups_ProfileId",
                table: "TemplateGroups",
                column: "ProfileId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Shapes");

            migrationBuilder.DropTable(
                name: "DrawingBoards");

            migrationBuilder.DropTable(
                name: "TemplateGroups");

            migrationBuilder.DropTable(
                name: "Profiles");
        }
    }
}
