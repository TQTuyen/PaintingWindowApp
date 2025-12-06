using PaintingApp.Models;
using Windows.Foundation;

namespace PaintingApp.Contracts;

public interface IShapeResizer
{
    void Resize(ShapeModel shape, ResizeHandle handle, Point anchorPoint, Point currentPoint);
    bool PreserveAspectRatio { get; }
}
