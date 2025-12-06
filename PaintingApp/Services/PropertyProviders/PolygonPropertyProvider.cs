using PaintingApp.Contracts;
using PaintingApp.Models;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using PropertyType = PaintingApp.Models.PropertyType;

namespace PaintingApp.Services.PropertyProviders
{
    public class PolygonPropertyProvider : IShapePropertyProvider
    {
        public List<EditableProperty> GetEditableProperties(ShapeModel shape)
        {
            if (shape is not PolygonModel polygon || polygon.Points.Count == 0)
                return new List<EditableProperty>();

            var bounds = polygon.GetBounds();
            var properties = new List<EditableProperty>
            {
                new("X", "X Position", bounds.X, PropertyType.Number, "Position"),
                new("Y", "Y Position", bounds.Y, PropertyType.Number, "Position"),
                new("Width", "Width", bounds.Width, PropertyType.Number, "Size") { MinimumValue = 10 },
                new("Height", "Height", bounds.Height, PropertyType.Number, "Size") { MinimumValue = 10 }
            };

            return properties;
        }

        public void SetProperty(ShapeModel shape, string propertyName, object value)
        {
            if (shape is not PolygonModel polygon || polygon.Points.Count == 0) return;

            var bounds = polygon.GetBounds();
            var newValue = (double)value;

            switch (propertyName)
            {
                case "X":
                {
                    var deltaX = newValue - bounds.X;
                    TranslatePoints(polygon, deltaX, 0);
                    break;
                }
                case "Y":
                {
                    var deltaY = newValue - bounds.Y;
                    TranslatePoints(polygon, 0, deltaY);
                    break;
                }
                case "Width":
                {
                    if (bounds.Width > 0 && newValue > 0)
                    {
                        var scaleX = newValue / bounds.Width;
                        ScalePoints(polygon, scaleX, 1, bounds.X, bounds.Y);
                    }
                    break;
                }
                case "Height":
                {
                    if (bounds.Height > 0 && newValue > 0)
                    {
                        var scaleY = newValue / bounds.Height;
                        ScalePoints(polygon, 1, scaleY, bounds.X, bounds.Y);
                    }
                    break;
                }
            }
        }

        private static void TranslatePoints(PolygonModel polygon, double deltaX, double deltaY)
        {
            var newPoints = polygon.Points
                .Select(p => new Point(p.X + deltaX, p.Y + deltaY))
                .ToList();
            polygon.Points = newPoints;
        }

        private static void ScalePoints(PolygonModel polygon, double scaleX, double scaleY, double originX, double originY)
        {
            var newPoints = polygon.Points
                .Select(p => new Point(
                    originX + (p.X - originX) * scaleX,
                    originY + (p.Y - originY) * scaleY))
                .ToList();
            polygon.Points = newPoints;
        }

        public bool HasProperty(string propertyName) =>
            propertyName is "X" or "Y" or "Width" or "Height";
    }
}
