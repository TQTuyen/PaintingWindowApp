using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PaintingApp.Data.Entities;

public class DrawingBoard
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public int Width { get; set; }

    [Required]
    public int Height { get; set; }

    [MaxLength(9)]
    public string BackgroundColor { get; set; } = "#FFFFFF";

    [Required]
    public int ProfileId { get; set; }

    [ForeignKey(nameof(ProfileId))]
    public virtual Profile Profile { get; set; } = null!;

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public DateTime LastModified { get; set; } = DateTime.UtcNow;

    public virtual ICollection<Shape> Shapes { get; set; }

    public DrawingBoard()
    {
        Shapes = new List<Shape>();
    }
}
