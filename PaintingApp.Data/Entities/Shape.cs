using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PaintingApp.Data.Entities;

public class Shape
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public ShapeType Type { get; set; }

    public int? DrawingBoardId { get; set; }

    [ForeignKey(nameof(DrawingBoardId))]
    public virtual DrawingBoard? DrawingBoard { get; set; }

    public int? TemplateGroupId { get; set; }

    [ForeignKey(nameof(TemplateGroupId))]
    public virtual TemplateGroup? TemplateGroup { get; set; }

    [Required]
    [MaxLength(9)]
    public string StrokeColor { get; set; } = "#000000";

    [Required]
    public double StrokeThickness { get; set; }

    [MaxLength(20)]
    public string StrokeStyle { get; set; } = "Solid";

    [MaxLength(9)]
    public string? FillColor { get; set; }

    [Required]
    [MaxLength(4000)]
    public string GeometryData { get; set; } = string.Empty;

    public int ZIndex { get; set; } = 0;

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
}
