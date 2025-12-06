using PaintingApp.Contracts;
using PaintingApp.Models;
using System.Collections.Generic;
using Windows.Foundation;
using PropertyType = PaintingApp.Models.PropertyType;

namespace PaintingApp.Services.PropertyProviders
{
    public class OvalPropertyProvider : IShapePropertyProvider
    {
        public List<EditableProperty> GetEditableProperties(ShapeModel shape)
        {
            if (shape is not OvalModel oval) return new List<EditableProperty>();
            // OvalModel stores center and radius; expose top-left/width/height for editing
            var topLeft = new Point(oval.Center.X - oval.RadiusX, oval.Center.Y - oval.RadiusY);
            var width = oval.RadiusX * 2;
            var height = oval.RadiusY * 2;

            return new List<EditableProperty>
            {
                new("X", "X Position", topLeft.X, PropertyType.Number, "Position"),
                new("Y", "Y Position", topLeft.Y, PropertyType.Number, "Position"),
                new("Width", "Width", width, PropertyType.Number, "Size") { MinimumValue = 10 },
                new("Height", "Height", height, PropertyType.Number, "Size") { MinimumValue = 10 }
            };
        }

        public void SetProperty(ShapeModel shape, string propertyName, object value)
        {
            if (shape is not OvalModel oval) return;

            switch (propertyName)
            {
                case "X":
                {
                    var newX = (double)value;
                    var centerX = newX + oval.RadiusX;
                    oval.Center = new Point(centerX, oval.Center.Y);
                    break;
                }
                case "Y":
                {
                    var newY = (double)value;
                    var centerY = newY + oval.RadiusY;
                    oval.Center = new Point(oval.Center.X, centerY);
                    break;
                }
                case "Width":
                {
                    var newWidth = (double)value;
                    oval.RadiusX = newWidth / 2.0;
                    break;
                }
                case "Height":
                {
                    var newHeight = (double)value;
                    oval.RadiusY = newHeight / 2.0;
                    break;
                }
            }
        }

        public bool HasProperty(string propertyName) =>
            propertyName is "X" or "Y" or "Width" or "Height";
    }
}
