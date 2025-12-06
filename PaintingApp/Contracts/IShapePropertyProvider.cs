using PaintingApp.Models;
using System.Collections.Generic;

namespace PaintingApp.Contracts
{
    public interface IShapePropertyProvider
    {
        List<EditableProperty> GetEditableProperties(ShapeModel shape);
        void SetProperty(ShapeModel shape, string propertyName, object value);
        bool HasProperty(string propertyName);
    }
}
