using System;
using System.Collections.Generic;
using System.Text.Json;
using PaintingApp.Contracts;
using PaintingApp.Data.Entities;
using PaintingApp.Models;
using Windows.Foundation;

namespace PaintingApp.Services.Adapters;

public class TriangleAdapter : BaseShapeAdapter, IShapeAdapter
{
    public Shape ToEntity(ShapeModel model)
    {
        if (model is not TriangleModel triangle)
            throw new ArgumentException("Model must be TriangleModel");

        var geometryData = new
        {
            Points = new List<PointData>
            {
                new() { X = triangle.Point1.X, Y = triangle.Point1.Y },
                new() { X = triangle.Point2.X, Y = triangle.Point2.Y },
                new() { X = triangle.Point3.X, Y = triangle.Point3.Y }
            }
        };

        return new Shape
        {
            Type = ShapeType.Triangle,
            StrokeColor = ColorToHex(triangle.StrokeColor),
            StrokeThickness = triangle.StrokeThickness,
            FillColor = ColorToHex(triangle.FillColor),
            ZIndex = triangle.ZIndex,
            GeometryData = JsonSerializer.Serialize(geometryData)
        };
    }

    public ShapeModel ToModel(Shape entity)
    {
        if (entity.Type != ShapeType.Triangle)
            throw new ArgumentException("Entity must be Triangle type");

        var geometryData = JsonSerializer.Deserialize<TriangleGeometryData>(entity.GeometryData);
        if (geometryData == null || geometryData.Points == null || geometryData.Points.Count < 3)
            throw new InvalidOperationException("Failed to deserialize triangle geometry data");

        return new TriangleModel
        {
            Point1 = new Point(geometryData.Points[0].X, geometryData.Points[0].Y),
            Point2 = new Point(geometryData.Points[1].X, geometryData.Points[1].Y),
            Point3 = new Point(geometryData.Points[2].X, geometryData.Points[2].Y),
            StrokeColor = ParseColor(entity.StrokeColor),
            StrokeThickness = entity.StrokeThickness,
            FillColor = ParseColor(entity.FillColor),
            ZIndex = entity.ZIndex
        };
    }

    private sealed class TriangleGeometryData
    {
        public List<PointData>? Points { get; set; }
    }

    private sealed class PointData
    {
        public double X { get; set; }
        public double Y { get; set; }
    }
}
