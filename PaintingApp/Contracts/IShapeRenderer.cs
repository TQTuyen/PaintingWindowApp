using Microsoft.UI.Xaml.Controls;
using PaintingApp.Models;

namespace PaintingApp.Contracts;

public interface IShapeRenderer
{
    void Render(Canvas canvas, ShapeModel shape);

    void Update(Canvas canvas, ShapeModel shape);

    void Remove(Canvas canvas, ShapeModel shape);
}
