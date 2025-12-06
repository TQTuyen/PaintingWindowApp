using PaintingApp.Contracts;
using PaintingApp.Models;
using System.Collections.Generic;
using Windows.Foundation;
using PropertyType = PaintingApp.Models.PropertyType;

namespace PaintingApp.Services.PropertyProviders
{
    public class CirclePropertyProvider : IShapePropertyProvider
    {
        public List<EditableProperty> GetEditableProperties(ShapeModel shape)
        {
            if (shape is not CircleModel circle) return new List<EditableProperty>();
            return new List<EditableProperty>
            {
                new("CenterX", "Center X", circle.Center.X, PropertyType.Number, "Position"),
                new("CenterY", "Center Y", circle.Center.Y, PropertyType.Number, "Position"),
                new("Radius", "Radius", circle.Radius, PropertyType.Number, "Size") { MinimumValue = 5 }
            };
        }

        public void SetProperty(ShapeModel shape, string propertyName, object value)
        {
            if (shape is not CircleModel circle) return;

            switch (propertyName)
            {
                case "CenterX":
                    circle.Center = new Point((double)value, circle.Center.Y);
                    break;
                case "CenterY":
                    circle.Center = new Point(circle.Center.X, (double)value);
                    break;
                case "Radius":
                    circle.Radius = (double)value;
                    break;
            }
        }

        public bool HasProperty(string propertyName) =>
            propertyName is "CenterX" or "CenterY" or "Radius";
    }
}
