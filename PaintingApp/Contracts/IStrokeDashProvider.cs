using Microsoft.UI.Xaml.Media;
using PaintingApp.Models;

namespace PaintingApp.Contracts;

public interface IStrokeDashProvider
{
    DoubleCollection? GetDashArray(StrokeDashStyle style);
}
