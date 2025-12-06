using PaintingApp.Contracts;
using PaintingApp.Models;
using System.Collections.Generic;
using Windows.Foundation;
using PropertyType = PaintingApp.Models.PropertyType;

namespace PaintingApp.Services.PropertyProviders
{
    public class RectanglePropertyProvider : IShapePropertyProvider
    {
        public List<EditableProperty> GetEditableProperties(ShapeModel shape)
        {
            if (shape is not RectangleModel rect) return new List<EditableProperty>();
            return new List<EditableProperty>
            {
                new("X", "X Position", rect.TopLeft.X, PropertyType.Number, "Position"),
                new("Y", "Y Position", rect.TopLeft.Y, PropertyType.Number, "Position"),
                new("Width", "Width", rect.Width, PropertyType.Number, "Size") { MinimumValue = 10 },
                new("Height", "Height", rect.Height, PropertyType.Number, "Size") { MinimumValue = 10 }
            };
        }

        public void SetProperty(ShapeModel shape, string propertyName, object value)
        {
            if (shape is not RectangleModel rect) return;

            switch (propertyName)
            {
                case "X":
                    rect.TopLeft = new Point((double)value, rect.TopLeft.Y);
                    break;
                case "Y":
                    rect.TopLeft = new Point(rect.TopLeft.X, (double)value);
                    break;
                case "Width":
                    rect.Width = (double)value;
                    break;
                case "Height":
                    rect.Height = (double)value;
                    break;
            }
        }

        public bool HasProperty(string propertyName) =>
            propertyName is "X" or "Y" or "Width" or "Height";
    }
}
