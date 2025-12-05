using Microsoft.EntityFrameworkCore;
using PaintingApp.Data.Entities;

namespace PaintingApp.Data;

public class AppDbContext : DbContext
{
    public virtual DbSet<Profile> Profiles { get; set; } = null!;
    public virtual DbSet<DrawingBoard> DrawingBoards { get; set; } = null!;
    public virtual DbSet<Shape> Shapes { get; set; } = null!;
    public virtual DbSet<TemplateGroup> TemplateGroups { get; set; } = null!;

    private static readonly DateTime SeedDate = new(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureProfile(modelBuilder);
        ConfigureDrawingBoard(modelBuilder);
        ConfigureTemplateGroup(modelBuilder);
        ConfigureShape(modelBuilder);

        SeedData(modelBuilder);
    }

    private static void ConfigureProfile(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Profile>(entity =>
        {
            entity.HasIndex(e => e.Name).IsUnique();

            entity.Property(e => e.Theme).HasDefaultValue("System");
            entity.Property(e => e.DefaultCanvasWidth).HasDefaultValue(800);
            entity.Property(e => e.DefaultCanvasHeight).HasDefaultValue(600);
            entity.Property(e => e.DefaultStrokeColor).HasDefaultValue("#000000");
            entity.Property(e => e.DefaultStrokeThickness).HasDefaultValue(2.0);
            entity.Property(e => e.DefaultStrokeStyle).HasDefaultValue("Solid");

            entity.HasMany(e => e.DrawingBoards)
                  .WithOne(e => e.Profile)
                  .HasForeignKey(e => e.ProfileId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.TemplateGroups)
                  .WithOne(e => e.Profile)
                  .HasForeignKey(e => e.ProfileId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureDrawingBoard(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DrawingBoard>(entity =>
        {
            entity.Property(e => e.BackgroundColor).HasDefaultValue("#FFFFFF");

            entity.HasOne(e => e.Profile)
                  .WithMany(e => e.DrawingBoards)
                  .HasForeignKey(e => e.ProfileId)
                  .IsRequired()
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.Shapes)
                  .WithOne(e => e.DrawingBoard)
                  .HasForeignKey(e => e.DrawingBoardId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureTemplateGroup(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TemplateGroup>(entity =>
        {
            entity.Property(e => e.UsageCount).HasDefaultValue(0);

            entity.HasOne(e => e.Profile)
                  .WithMany(e => e.TemplateGroups)
                  .HasForeignKey(e => e.ProfileId)
                  .IsRequired()
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.Shapes)
                  .WithOne(e => e.TemplateGroup)
                  .HasForeignKey(e => e.TemplateGroupId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureShape(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Shape>(entity =>
        {
            entity.Property(e => e.Type).HasConversion<string>();
            entity.Property(e => e.ZIndex).HasDefaultValue(0);
            entity.Property(e => e.StrokeStyle).HasDefaultValue("Solid");

            entity.HasOne(e => e.DrawingBoard)
                  .WithMany(e => e.Shapes)
                  .HasForeignKey(e => e.DrawingBoardId)
                  .IsRequired(false)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.TemplateGroup)
                  .WithMany(e => e.Shapes)
                  .HasForeignKey(e => e.TemplateGroupId)
                  .IsRequired(false)
                  .OnDelete(DeleteBehavior.Cascade);

            // Shape must belong to exactly one parent: either a DrawingBoard OR a TemplateGroup, never both or neither
            entity.ToTable(t => t.HasCheckConstraint(
                "CK_Shape_ParentConstraint",
                "([DrawingBoardId] IS NOT NULL AND [TemplateGroupId] IS NULL) OR ([DrawingBoardId] IS NULL AND [TemplateGroupId] IS NOT NULL)"));
        });
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        SeedProfiles(modelBuilder);
        SeedTemplateGroups(modelBuilder);
        SeedShapes(modelBuilder);
    }

    private static void SeedProfiles(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Profile>().HasData(
            new Profile
            {
                Id = 1,
                Name = "Default User",
                Description = "Default profile for quick start",
                Theme = "System",
                DefaultCanvasWidth = 800,
                DefaultCanvasHeight = 600,
                DefaultStrokeColor = "#000000",
                DefaultStrokeThickness = 2.0,
                DefaultStrokeStyle = "Solid",
                CreatedDate = SeedDate
            },
            new Profile
            {
                Id = 2,
                Name = "Dark Theme User",
                Description = "Profile optimized for dark theme",
                Theme = "Dark",
                DefaultCanvasWidth = 1024,
                DefaultCanvasHeight = 768,
                DefaultStrokeColor = "#FFFFFF",
                DefaultStrokeThickness = 3.0,
                DefaultStrokeStyle = "Solid",
                CreatedDate = SeedDate
            },
            new Profile
            {
                Id = 3,
                Name = "Large Canvas User",
                Description = "Profile for detailed artwork",
                Theme = "Light",
                DefaultCanvasWidth = 1920,
                DefaultCanvasHeight = 1080,
                DefaultStrokeColor = "#FF5722",
                DefaultStrokeThickness = 1.5,
                DefaultStrokeStyle = "Solid",
                CreatedDate = SeedDate
            }
        );
    }

    private static void SeedTemplateGroups(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TemplateGroup>().HasData(
            new TemplateGroup
            {
                Id = 1,
                Name = "Star",
                Description = "5-pointed star shape",
                ProfileId = 1,
                UsageCount = 0,
                CreatedDate = SeedDate
            },
            new TemplateGroup
            {
                Id = 2,
                Name = "House",
                Description = "Simple house with roof",
                ProfileId = 1,
                UsageCount = 0,
                CreatedDate = SeedDate
            },
            new TemplateGroup
            {
                Id = 3,
                Name = "Arrow",
                Description = "Right-pointing arrow",
                ProfileId = 2,
                UsageCount = 0,
                CreatedDate = SeedDate
            },
            new TemplateGroup
            {
                Id = 4,
                Name = "Smiley Face",
                Description = "Happy face emoji style",
                ProfileId = 2,
                UsageCount = 0,
                CreatedDate = SeedDate
            },
            new TemplateGroup
            {
                Id = 5,
                Name = "Basic Shapes Set",
                Description = "Collection of basic geometric shapes",
                ProfileId = 3,
                UsageCount = 0,
                CreatedDate = SeedDate
            }
        );
    }

    private static void SeedShapes(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Shape>().HasData(
            // Star template - 10-point polygon (5-pointed star)
            new Shape
            {
                Id = 1,
                Type = ShapeType.Polygon,
                TemplateGroupId = 1,
                StrokeColor = "#FFD700",
                StrokeThickness = 2.0,
                StrokeStyle = "Solid",
                FillColor = "#FFD700",
                GeometryData = "{\"Points\":[{\"X\":50,\"Y\":0},{\"X\":61,\"Y\":35},{\"X\":98,\"Y\":35},{\"X\":68,\"Y\":57},{\"X\":79,\"Y\":91},{\"X\":50,\"Y\":70},{\"X\":21,\"Y\":91},{\"X\":32,\"Y\":57},{\"X\":2,\"Y\":35},{\"X\":39,\"Y\":35}]}",
                ZIndex = 0,
                CreatedDate = SeedDate
            },
            // House template - Rectangle (walls)
            new Shape
            {
                Id = 2,
                Type = ShapeType.Rectangle,
                TemplateGroupId = 2,
                StrokeColor = "#8B4513",
                StrokeThickness = 2.0,
                StrokeStyle = "Solid",
                FillColor = "#DEB887",
                GeometryData = "{\"X\":20,\"Y\":50,\"Width\":60,\"Height\":45}",
                ZIndex = 0,
                CreatedDate = SeedDate
            },
            // House template - Triangle (roof)
            new Shape
            {
                Id = 3,
                Type = ShapeType.Triangle,
                TemplateGroupId = 2,
                StrokeColor = "#8B0000",
                StrokeThickness = 2.0,
                StrokeStyle = "Solid",
                FillColor = "#B22222",
                GeometryData = "{\"Points\":[{\"X\":50,\"Y\":10},{\"X\":85,\"Y\":50},{\"X\":15,\"Y\":50}]}",
                ZIndex = 1,
                CreatedDate = SeedDate
            },
            // Arrow template - Rectangle (body)
            new Shape
            {
                Id = 4,
                Type = ShapeType.Rectangle,
                TemplateGroupId = 3,
                StrokeColor = "#2196F3",
                StrokeThickness = 2.0,
                StrokeStyle = "Solid",
                FillColor = "#64B5F6",
                GeometryData = "{\"X\":10,\"Y\":35,\"Width\":50,\"Height\":30}",
                ZIndex = 0,
                CreatedDate = SeedDate
            },
            // Arrow template - Triangle (head)
            new Shape
            {
                Id = 5,
                Type = ShapeType.Triangle,
                TemplateGroupId = 3,
                StrokeColor = "#2196F3",
                StrokeThickness = 2.0,
                StrokeStyle = "Solid",
                FillColor = "#64B5F6",
                GeometryData = "{\"Points\":[{\"X\":60,\"Y\":15},{\"X\":95,\"Y\":50},{\"X\":60,\"Y\":85}]}",
                ZIndex = 1,
                CreatedDate = SeedDate
            },
            // Smiley Face template - Circle (face)
            new Shape
            {
                Id = 6,
                Type = ShapeType.Circle,
                TemplateGroupId = 4,
                StrokeColor = "#000000",
                StrokeThickness = 2.0,
                StrokeStyle = "Solid",
                FillColor = "#FFEB3B",
                GeometryData = "{\"CenterX\":50,\"CenterY\":50,\"Radius\":40}",
                ZIndex = 0,
                CreatedDate = SeedDate
            },
            // Smiley Face template - Circle (left eye)
            new Shape
            {
                Id = 7,
                Type = ShapeType.Circle,
                TemplateGroupId = 4,
                StrokeColor = "#000000",
                StrokeThickness = 1.0,
                StrokeStyle = "Solid",
                FillColor = "#000000",
                GeometryData = "{\"CenterX\":35,\"CenterY\":40,\"Radius\":5}",
                ZIndex = 1,
                CreatedDate = SeedDate
            },
            // Smiley Face template - Circle (right eye)
            new Shape
            {
                Id = 8,
                Type = ShapeType.Circle,
                TemplateGroupId = 4,
                StrokeColor = "#000000",
                StrokeThickness = 1.0,
                StrokeStyle = "Solid",
                FillColor = "#000000",
                GeometryData = "{\"CenterX\":65,\"CenterY\":40,\"Radius\":5}",
                ZIndex = 1,
                CreatedDate = SeedDate
            },
            // Smiley Face template - Oval (smile)
            new Shape
            {
                Id = 9,
                Type = ShapeType.Oval,
                TemplateGroupId = 4,
                StrokeColor = "#000000",
                StrokeThickness = 2.0,
                StrokeStyle = "Solid",
                FillColor = null,
                GeometryData = "{\"CenterX\":50,\"CenterY\":62,\"RadiusX\":20,\"RadiusY\":10}",
                ZIndex = 1,
                CreatedDate = SeedDate
            },
            // Basic Shapes Set - Circle
            new Shape
            {
                Id = 10,
                Type = ShapeType.Circle,
                TemplateGroupId = 5,
                StrokeColor = "#E91E63",
                StrokeThickness = 2.0,
                StrokeStyle = "Solid",
                FillColor = "#F48FB1",
                GeometryData = "{\"CenterX\":25,\"CenterY\":50,\"Radius\":20}",
                ZIndex = 0,
                CreatedDate = SeedDate
            },
            // Basic Shapes Set - Rectangle
            new Shape
            {
                Id = 11,
                Type = ShapeType.Rectangle,
                TemplateGroupId = 5,
                StrokeColor = "#4CAF50",
                StrokeThickness = 2.0,
                StrokeStyle = "Solid",
                FillColor = "#A5D6A7",
                GeometryData = "{\"X\":55,\"Y\":30,\"Width\":35,\"Height\":40}",
                ZIndex = 0,
                CreatedDate = SeedDate
            },
            // Basic Shapes Set - Triangle
            new Shape
            {
                Id = 12,
                Type = ShapeType.Triangle,
                TemplateGroupId = 5,
                StrokeColor = "#9C27B0",
                StrokeThickness = 2.0,
                StrokeStyle = "Solid",
                FillColor = "#CE93D8",
                GeometryData = "{\"Points\":[{\"X\":50,\"Y\":5},{\"X\":70,\"Y\":25},{\"X\":30,\"Y\":25}]}",
                ZIndex = 0,
                CreatedDate = SeedDate
            }
        );
    }
}
