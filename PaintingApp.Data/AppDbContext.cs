using Microsoft.EntityFrameworkCore;
using PaintingApp.Data.Entities;

namespace PaintingApp.Data;

public class AppDbContext : DbContext
{
    public virtual DbSet<Profile> Profiles { get; set; } = null!;
    public virtual DbSet<DrawingBoard> DrawingBoards { get; set; } = null!;
    public virtual DbSet<Shape> Shapes { get; set; } = null!;
    public virtual DbSet<TemplateGroup> TemplateGroups { get; set; } = null!;

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
}
