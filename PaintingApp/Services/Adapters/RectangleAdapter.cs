using System;
using System.Text.Json;
using PaintingApp.Contracts;
using PaintingApp.Data.Entities;
using PaintingApp.Models;
using Windows.Foundation;

namespace PaintingApp.Services.Adapters;

public class RectangleAdapter : BaseShapeAdapter, IShapeAdapter
{
    public Shape ToEntity(ShapeModel model)
    {
        if (model is not RectangleModel rect)
            throw new ArgumentException("Model must be RectangleModel");

        var geometryData = new
        {
            X = rect.TopLeft.X,
            Y = rect.TopLeft.Y,
            rect.Width,
            rect.Height
        };

        return new Shape
        {
            Type = ShapeType.Rectangle,
            StrokeColor = ColorToHex(rect.StrokeColor),
            StrokeThickness = rect.StrokeThickness,
            FillColor = ColorToHex(rect.FillColor),
            ZIndex = rect.ZIndex,
            GeometryData = JsonSerializer.Serialize(geometryData)
        };
    }

    public ShapeModel ToModel(Shape entity)
    {
        if (entity.Type != ShapeType.Rectangle)
            throw new ArgumentException("Entity must be Rectangle type");

        var geometryData = JsonSerializer.Deserialize<RectangleGeometryData>(entity.GeometryData);
        if (geometryData == null)
            throw new InvalidOperationException("Failed to deserialize rectangle geometry data");

        return new RectangleModel
        {
            TopLeft = new Point(geometryData.X, geometryData.Y),
            Width = geometryData.Width,
            Height = geometryData.Height,
            StrokeColor = ParseColor(entity.StrokeColor),
            StrokeThickness = entity.StrokeThickness,
            FillColor = ParseColor(entity.FillColor),
            ZIndex = entity.ZIndex
        };
    }

    private sealed class RectangleGeometryData
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
    }
}
