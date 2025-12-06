using System;
using Microsoft.UI;
using PaintingApp.Models;
using Windows.UI;

namespace PaintingApp.Services.Adapters;

public abstract class BaseShapeAdapter
{
    protected static string ColorToHex(Color color)
    {
        return $"#{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2}";
    }

    protected static Color ParseColor(string? hex)
    {
        if (string.IsNullOrEmpty(hex)) return Colors.Transparent;

        hex = hex.TrimStart('#');
        if (hex.Length == 6) hex = "FF" + hex;

        if (hex.Length == 8)
        {
            return Color.FromArgb(
                Convert.ToByte(hex.Substring(0, 2), 16),
                Convert.ToByte(hex.Substring(2, 2), 16),
                Convert.ToByte(hex.Substring(4, 2), 16),
                Convert.ToByte(hex.Substring(6, 2), 16)
            );
        }

        return Colors.Transparent;
    }

    protected static StrokeDashStyle ParseStrokeDashStyle(string? strokeStyle)
    {
        if (string.IsNullOrEmpty(strokeStyle))
            return StrokeDashStyle.Solid;

        return Enum.TryParse<StrokeDashStyle>(strokeStyle, true, out var result)
            ? result
            : StrokeDashStyle.Solid;
    }
}
