using System;
using System.Text.Json;
using PaintingApp.Contracts;
using PaintingApp.Data.Entities;
using PaintingApp.Models;
using Windows.Foundation;

namespace PaintingApp.Services.Adapters;

public class CircleAdapter : BaseShapeAdapter, IShapeAdapter
{
    public Shape ToEntity(ShapeModel model)
    {
        if (model is not CircleModel circle)
            throw new ArgumentException("Model must be CircleModel");

        var geometryData = new
        {
            CenterX = circle.Center.X,
            CenterY = circle.Center.Y,
            circle.Radius
        };

        return new Shape
        {
            Type = ShapeType.Circle,
            StrokeColor = ColorToHex(circle.StrokeColor),
            StrokeThickness = circle.StrokeThickness,
            FillColor = ColorToHex(circle.FillColor),
            ZIndex = circle.ZIndex,
            GeometryData = JsonSerializer.Serialize(geometryData)
        };
    }

    public ShapeModel ToModel(Shape entity)
    {
        if (entity.Type != ShapeType.Circle)
            throw new ArgumentException("Entity must be Circle type");

        var geometryData = JsonSerializer.Deserialize<CircleGeometryData>(entity.GeometryData);
        if (geometryData == null)
            throw new InvalidOperationException("Failed to deserialize circle geometry data");

        return new CircleModel
        {
            Center = new Point(geometryData.CenterX, geometryData.CenterY),
            Radius = geometryData.Radius,
            StrokeColor = ParseColor(entity.StrokeColor),
            StrokeThickness = entity.StrokeThickness,
            FillColor = ParseColor(entity.FillColor),
            ZIndex = entity.ZIndex
        };
    }

    private sealed class CircleGeometryData
    {
        public double CenterX { get; set; }
        public double CenterY { get; set; }
        public double Radius { get; set; }
    }
}
