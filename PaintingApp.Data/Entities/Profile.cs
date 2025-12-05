using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PaintingApp.Data.Entities;

public class Profile
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [MaxLength(20)]
    public string Theme { get; set; } = "System";

    public int DefaultCanvasWidth { get; set; } = 800;

    public int DefaultCanvasHeight { get; set; } = 600;

    [MaxLength(9)]
    public string DefaultStrokeColor { get; set; } = "#000000";

    public double DefaultStrokeThickness { get; set; } = 2.0;

    [MaxLength(20)]
    public string DefaultStrokeStyle { get; set; } = "Solid";

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public virtual ICollection<DrawingBoard> DrawingBoards { get; set; }

    public virtual ICollection<TemplateGroup> TemplateGroups { get; set; }

    public Profile()
    {
        DrawingBoards = new List<DrawingBoard>();
        TemplateGroups = new List<TemplateGroup>();
    }
}
