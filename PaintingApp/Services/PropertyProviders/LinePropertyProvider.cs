using PaintingApp.Contracts;
using PaintingApp.Models;
using System.Collections.Generic;
using Windows.Foundation;
using PropertyType = PaintingApp.Models.PropertyType;

namespace PaintingApp.Services.PropertyProviders
{
    public class LinePropertyProvider : IShapePropertyProvider
    {
        public List<EditableProperty> GetEditableProperties(ShapeModel shape)
        {
            if (shape is not LineModel line) return new List<EditableProperty>();
            return new List<EditableProperty>
            {
                new("X1", "Start X", line.StartPoint.X, PropertyType.Number, "Start Point"),
                new("Y1", "Start Y", line.StartPoint.Y, PropertyType.Number, "Start Point"),
                new("X2", "End X", line.EndPoint.X, PropertyType.Number, "End Point"),
                new("Y2", "End Y", line.EndPoint.Y, PropertyType.Number, "End Point")
            };
        }

        public void SetProperty(ShapeModel shape, string propertyName, object value)
        {
            if (shape is not LineModel line) return;

            switch (propertyName)
            {
                case "X1":
                    line.StartPoint = new Point((double)value, line.StartPoint.Y);
                    break;
                case "Y1":
                    line.StartPoint = new Point(line.StartPoint.X, (double)value);
                    break;
                case "X2":
                    line.EndPoint = new Point((double)value, line.EndPoint.Y);
                    break;
                case "Y2":
                    line.EndPoint = new Point(line.EndPoint.X, (double)value);
                    break;
            }
        }

        public bool HasProperty(string propertyName) =>
            propertyName is "X1" or "Y1" or "X2" or "Y2";
    }
}
