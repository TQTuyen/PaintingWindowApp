using Microsoft.UI;
using Windows.UI;

namespace PaintingApp.Models;

public class TemplateUsageItem
{
    public int Id { get; set; }
    public string TemplateName { get; set; } = string.Empty;
    public int UsageCount { get; set; }
    public double Percentage { get; set; }
    public Color Color { get; set; }
    public string ColorHex => $"#{Color.R:X2}{Color.G:X2}{Color.B:X2}";

    private static readonly Color[] ChartColors =
    [
        Color.FromArgb(255, 0, 120, 215),    // Blue
        Color.FromArgb(255, 16, 137, 62),    // Green
        Color.FromArgb(255, 255, 140, 0),    // Orange
        Color.FromArgb(255, 232, 17, 35),    // Red
        Color.FromArgb(255, 136, 23, 152),   // Purple
    ];

    public static Color GetColorForIndex(int index)
    {
        return ChartColors[index % ChartColors.Length];
    }
}
