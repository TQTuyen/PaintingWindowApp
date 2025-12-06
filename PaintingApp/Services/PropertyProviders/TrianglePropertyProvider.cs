using PaintingApp.Contracts;
using PaintingApp.Models;
using System.Collections.Generic;
using Windows.Foundation;
using PropertyType = PaintingApp.Models.PropertyType;

namespace PaintingApp.Services.PropertyProviders
{
    public class TrianglePropertyProvider : IShapePropertyProvider
    {
        public List<EditableProperty> GetEditableProperties(ShapeModel shape)
        {
            if (shape is not TriangleModel triangle) return new List<EditableProperty>();

            return new List<EditableProperty>
            {
                new("X1", "Point 1 X", triangle.Point1.X, PropertyType.Number, "Point 1"),
                new("Y1", "Point 1 Y", triangle.Point1.Y, PropertyType.Number, "Point 1"),
                new("X2", "Point 2 X", triangle.Point2.X, PropertyType.Number, "Point 2"),
                new("Y2", "Point 2 Y", triangle.Point2.Y, PropertyType.Number, "Point 2"),
                new("X3", "Point 3 X", triangle.Point3.X, PropertyType.Number, "Point 3"),
                new("Y3", "Point 3 Y", triangle.Point3.Y, PropertyType.Number, "Point 3")
            };
        }

        public void SetProperty(ShapeModel shape, string propertyName, object value)
        {
            if (shape is not TriangleModel triangle) return;

            switch (propertyName)
            {
                case "X1":
                    triangle.Point1 = new Point((double)value, triangle.Point1.Y);
                    break;
                case "Y1":
                    triangle.Point1 = new Point(triangle.Point1.X, (double)value);
                    break;
                case "X2":
                    triangle.Point2 = new Point((double)value, triangle.Point2.Y);
                    break;
                case "Y2":
                    triangle.Point2 = new Point(triangle.Point2.X, (double)value);
                    break;
                case "X3":
                    triangle.Point3 = new Point((double)value, triangle.Point3.Y);
                    break;
                case "Y3":
                    triangle.Point3 = new Point(triangle.Point3.X, (double)value);
                    break;
            }
        }

        public bool HasProperty(string propertyName) =>
            propertyName is "X1" or "Y1" or "X2" or "Y2" or "X3" or "Y3";
    }
}
