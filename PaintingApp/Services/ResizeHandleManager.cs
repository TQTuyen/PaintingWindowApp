using System.Collections.Generic;
using Microsoft.UI;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using PaintingApp.Models;
using Windows.Foundation;

namespace PaintingApp.Services;

public class ResizeHandleManager
{
    private const double HandleSize = 8;
    private readonly List<Ellipse> _handles = [];
    private readonly Dictionary<Ellipse, ResizeHandle> _handleMap = [];

    public void ShowHandles(Canvas canvas, Rect bounds)
    {
        HideHandles(canvas);

        var positions = new Dictionary<ResizeHandle, Point>
        {
            { ResizeHandle.TopLeft, new Point(bounds.Left, bounds.Top) },
            { ResizeHandle.TopCenter, new Point(bounds.Left + bounds.Width / 2, bounds.Top) },
            { ResizeHandle.TopRight, new Point(bounds.Right, bounds.Top) },
            { ResizeHandle.MiddleLeft, new Point(bounds.Left, bounds.Top + bounds.Height / 2) },
            { ResizeHandle.MiddleRight, new Point(bounds.Right, bounds.Top + bounds.Height / 2) },
            { ResizeHandle.BottomLeft, new Point(bounds.Left, bounds.Bottom) },
            { ResizeHandle.BottomCenter, new Point(bounds.Left + bounds.Width / 2, bounds.Bottom) },
            { ResizeHandle.BottomRight, new Point(bounds.Right, bounds.Bottom) }
        };

        foreach (var kvp in positions)
        {
            var handle = new Ellipse
            {
                Width = HandleSize,
                Height = HandleSize,
                Fill = new SolidColorBrush(Colors.White),
                Stroke = new SolidColorBrush(Colors.DodgerBlue),
                StrokeThickness = 2
            };

            Canvas.SetLeft(handle, kvp.Value.X - HandleSize / 2);
            Canvas.SetTop(handle, kvp.Value.Y - HandleSize / 2);
            Canvas.SetZIndex(handle, 10001);

            canvas.Children.Add(handle);
            _handles.Add(handle);
            _handleMap[handle] = kvp.Key;
        }
    }

    public void HideHandles(Canvas canvas)
    {
        foreach (var handle in _handles)
        {
            canvas.Children.Remove(handle);
        }
        _handles.Clear();
        _handleMap.Clear();
    }

    public ResizeHandle HitTestHandle(Point point)
    {
        foreach (var kvp in _handleMap)
        {
            var handle = kvp.Key;
            var left = Canvas.GetLeft(handle);
            var top = Canvas.GetTop(handle);
            var bounds = new Rect(left - 2, top - 2, HandleSize + 4, HandleSize + 4);

            if (bounds.Contains(point))
            {
                return kvp.Value;
            }
        }

        return ResizeHandle.None;
    }
}
