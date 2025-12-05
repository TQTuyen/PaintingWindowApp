using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PaintingApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedInitialData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Profiles",
                columns: new[] { "Id", "CreatedDate", "DefaultCanvasHeight", "DefaultCanvasWidth", "DefaultStrokeColor", "DefaultStrokeStyle", "DefaultStrokeThickness", "Description", "Name", "Theme" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 600, 800, "#000000", "Solid", 2.0, "Default profile for quick start", "Default User", "System" },
                    { 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 768, 1024, "#FFFFFF", "Solid", 3.0, "Profile optimized for dark theme", "Dark Theme User", "Dark" },
                    { 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1080, 1920, "#FF5722", "Solid", 1.5, "Profile for detailed artwork", "Large Canvas User", "Light" }
                });

            migrationBuilder.InsertData(
                table: "TemplateGroups",
                columns: new[] { "Id", "CreatedDate", "Description", "Name", "ProfileId" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "5-pointed star shape", "Star", 1 },
                    { 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Simple house with roof", "House", 1 },
                    { 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Right-pointing arrow", "Arrow", 2 },
                    { 4, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Happy face emoji style", "Smiley Face", 2 },
                    { 5, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Collection of basic geometric shapes", "Basic Shapes Set", 3 }
                });

            migrationBuilder.InsertData(
                table: "Shapes",
                columns: new[] { "Id", "CreatedDate", "DrawingBoardId", "FillColor", "GeometryData", "StrokeColor", "StrokeStyle", "StrokeThickness", "TemplateGroupId", "Type" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "#FFD700", "{\"Points\":[{\"X\":50,\"Y\":0},{\"X\":61,\"Y\":35},{\"X\":98,\"Y\":35},{\"X\":68,\"Y\":57},{\"X\":79,\"Y\":91},{\"X\":50,\"Y\":70},{\"X\":21,\"Y\":91},{\"X\":32,\"Y\":57},{\"X\":2,\"Y\":35},{\"X\":39,\"Y\":35}]}", "#FFD700", "Solid", 2.0, 1, "Polygon" },
                    { 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "#DEB887", "{\"X\":20,\"Y\":50,\"Width\":60,\"Height\":45}", "#8B4513", "Solid", 2.0, 2, "Rectangle" }
                });

            migrationBuilder.InsertData(
                table: "Shapes",
                columns: new[] { "Id", "CreatedDate", "DrawingBoardId", "FillColor", "GeometryData", "StrokeColor", "StrokeStyle", "StrokeThickness", "TemplateGroupId", "Type", "ZIndex" },
                values: new object[] { 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "#B22222", "{\"Points\":[{\"X\":50,\"Y\":10},{\"X\":85,\"Y\":50},{\"X\":15,\"Y\":50}]}", "#8B0000", "Solid", 2.0, 2, "Triangle", 1 });

            migrationBuilder.InsertData(
                table: "Shapes",
                columns: new[] { "Id", "CreatedDate", "DrawingBoardId", "FillColor", "GeometryData", "StrokeColor", "StrokeStyle", "StrokeThickness", "TemplateGroupId", "Type" },
                values: new object[] { 4, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "#64B5F6", "{\"X\":10,\"Y\":35,\"Width\":50,\"Height\":30}", "#2196F3", "Solid", 2.0, 3, "Rectangle" });

            migrationBuilder.InsertData(
                table: "Shapes",
                columns: new[] { "Id", "CreatedDate", "DrawingBoardId", "FillColor", "GeometryData", "StrokeColor", "StrokeStyle", "StrokeThickness", "TemplateGroupId", "Type", "ZIndex" },
                values: new object[] { 5, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "#64B5F6", "{\"Points\":[{\"X\":60,\"Y\":15},{\"X\":95,\"Y\":50},{\"X\":60,\"Y\":85}]}", "#2196F3", "Solid", 2.0, 3, "Triangle", 1 });

            migrationBuilder.InsertData(
                table: "Shapes",
                columns: new[] { "Id", "CreatedDate", "DrawingBoardId", "FillColor", "GeometryData", "StrokeColor", "StrokeStyle", "StrokeThickness", "TemplateGroupId", "Type" },
                values: new object[] { 6, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "#FFEB3B", "{\"CenterX\":50,\"CenterY\":50,\"Radius\":40}", "#000000", "Solid", 2.0, 4, "Circle" });

            migrationBuilder.InsertData(
                table: "Shapes",
                columns: new[] { "Id", "CreatedDate", "DrawingBoardId", "FillColor", "GeometryData", "StrokeColor", "StrokeStyle", "StrokeThickness", "TemplateGroupId", "Type", "ZIndex" },
                values: new object[,]
                {
                    { 7, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "#000000", "{\"CenterX\":35,\"CenterY\":40,\"Radius\":5}", "#000000", "Solid", 1.0, 4, "Circle", 1 },
                    { 8, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "#000000", "{\"CenterX\":65,\"CenterY\":40,\"Radius\":5}", "#000000", "Solid", 1.0, 4, "Circle", 1 },
                    { 9, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "{\"CenterX\":50,\"CenterY\":62,\"RadiusX\":20,\"RadiusY\":10}", "#000000", "Solid", 2.0, 4, "Oval", 1 }
                });

            migrationBuilder.InsertData(
                table: "Shapes",
                columns: new[] { "Id", "CreatedDate", "DrawingBoardId", "FillColor", "GeometryData", "StrokeColor", "StrokeStyle", "StrokeThickness", "TemplateGroupId", "Type" },
                values: new object[,]
                {
                    { 10, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "#F48FB1", "{\"CenterX\":25,\"CenterY\":50,\"Radius\":20}", "#E91E63", "Solid", 2.0, 5, "Circle" },
                    { 11, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "#A5D6A7", "{\"X\":55,\"Y\":30,\"Width\":35,\"Height\":40}", "#4CAF50", "Solid", 2.0, 5, "Rectangle" },
                    { 12, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "#CE93D8", "{\"Points\":[{\"X\":50,\"Y\":5},{\"X\":70,\"Y\":25},{\"X\":30,\"Y\":25}]}", "#9C27B0", "Solid", 2.0, 5, "Triangle" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Shapes",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Shapes",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Shapes",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Shapes",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Shapes",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Shapes",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Shapes",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Shapes",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Shapes",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Shapes",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Shapes",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Shapes",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "TemplateGroups",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "TemplateGroups",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "TemplateGroups",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "TemplateGroups",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "TemplateGroups",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Profiles",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Profiles",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Profiles",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
