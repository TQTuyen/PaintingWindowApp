using Microsoft.UI.Xaml.Media;
using PaintingApp.Contracts;
using PaintingApp.Models;

namespace PaintingApp.Services;

public class StrokeDashProvider : IStrokeDashProvider
{
    public DoubleCollection? GetDashArray(StrokeDashStyle style)
    {
        return style switch
        {
            StrokeDashStyle.Solid => null,
            StrokeDashStyle.Dash => [4, 2],
            StrokeDashStyle.Dot => [1, 2],
            StrokeDashStyle.DashDot => [4, 2, 1, 2],
            StrokeDashStyle.DashDotDot => [4, 2, 1, 2, 1, 2],
            _ => null
        };
    }
}
