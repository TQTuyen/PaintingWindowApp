using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PaintingApp.Data.Entities;

public class TemplateGroup
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(300)]
    public string? Description { get; set; }

    [Required]
    public int ProfileId { get; set; }

    [ForeignKey(nameof(ProfileId))]
    public virtual Profile Profile { get; set; } = null!;

    public int UsageCount { get; set; } = 0;

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public virtual ICollection<Shape> Shapes { get; set; }

    public TemplateGroup()
    {
        Shapes = new List<Shape>();
    }
}
