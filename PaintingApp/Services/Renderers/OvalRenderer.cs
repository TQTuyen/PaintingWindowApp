using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using PaintingApp.Contracts;
using PaintingApp.Models;

namespace PaintingApp.Services.Renderers;

public class OvalRenderer : IShapeRenderer
{
    public void Render(Canvas canvas, ShapeModel shape)
    {
        if (shape is not OvalModel oval) return;

        var uiEllipse = new Ellipse
        {
            Width = oval.RadiusX * 2,
            Height = oval.RadiusY * 2,
            Stroke = new SolidColorBrush(oval.StrokeColor),
            StrokeThickness = oval.StrokeThickness,
            Fill = new SolidColorBrush(oval.FillColor),
            Tag = shape
        };

        Canvas.SetLeft(uiEllipse, oval.Center.X - oval.RadiusX);
        Canvas.SetTop(uiEllipse, oval.Center.Y - oval.RadiusY);
        Canvas.SetZIndex(uiEllipse, oval.ZIndex);

        canvas.Children.Add(uiEllipse);
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
