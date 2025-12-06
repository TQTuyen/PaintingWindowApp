using PaintingApp.Contracts;
using PaintingApp.Models;
using Windows.Foundation;

namespace PaintingApp.Services.Resizers;

public class LineResizer : IShapeResizer
{
    public bool PreserveAspectRatio => false;

    public void Resize(ShapeModel shape, ResizeHandle handle, Point anchorPoint, Point currentPoint)
    {
        if (shape is not LineModel line) return;

        switch (handle)
        {
            case ResizeHandle.TopLeft:
            case ResizeHandle.MiddleLeft:
            case ResizeHandle.BottomLeft:
                line.StartPoint = currentPoint;
                break;

            case ResizeHandle.TopRight:
            case ResizeHandle.MiddleRight:
            case ResizeHandle.BottomRight:
                line.EndPoint = currentPoint;
                break;
        }
    }
}
