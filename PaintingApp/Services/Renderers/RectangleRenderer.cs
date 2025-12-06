using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using PaintingApp.Contracts;
using PaintingApp.Models;

namespace PaintingApp.Services.Renderers;

public class RectangleRenderer : IShapeRenderer
{
    public void Render(Canvas canvas, ShapeModel shape)
    {
        if (shape is not RectangleModel rectangle) return;

        var uiRect = new Rectangle
        {
            Width = rectangle.Width,
            Height = rectangle.Height,
            Stroke = new SolidColorBrush(rectangle.StrokeColor),
            StrokeThickness = rectangle.StrokeThickness,
            Fill = new SolidColorBrush(rectangle.FillColor),
            Tag = shape
        };

        Canvas.SetLeft(uiRect, rectangle.TopLeft.X);
        Canvas.SetTop(uiRect, rectangle.TopLeft.Y);
        Canvas.SetZIndex(uiRect, rectangle.ZIndex);

        canvas.Children.Add(uiRect);
    }

    public void Update(Canvas canvas, ShapeModel shape)
    {
        Remove(canvas, shape);
        Render(canvas, shape);
    }

    public void Remove(Canvas canvas, ShapeModel shape)
    {
        var element = canvas.Children.FirstOrDefault(c => (c as FrameworkElement)?.Tag == shape);
        if (element != null)
        {
            canvas.Children.Remove(element);
        }
    }
}
