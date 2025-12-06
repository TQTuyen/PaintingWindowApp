using PaintingApp.Data.Entities;
using PaintingApp.Models;

namespace PaintingApp.Contracts;

public interface IShapeAdapter
{
    Shape ToEntity(ShapeModel model);

    ShapeModel ToModel(Shape entity);
}
