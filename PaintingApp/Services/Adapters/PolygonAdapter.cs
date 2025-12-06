using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using PaintingApp.Contracts;
using PaintingApp.Data.Entities;
using PaintingApp.Models;
using Windows.Foundation;

namespace PaintingApp.Services.Adapters;

public class PolygonAdapter : BaseShapeAdapter, IShapeAdapter
{
    public Shape ToEntity(ShapeModel model)
    {
        if (model is not PolygonModel polygon)
            throw new ArgumentException("Model must be PolygonModel");

        var geometryData = new
        {
            Points = polygon.Points.Select(p => new PointData { X = p.X, Y = p.Y }).ToList()
        };

        return new Shape
        {
            Type = ShapeType.Polygon,
            StrokeColor = ColorToHex(polygon.StrokeColor),
            StrokeThickness = polygon.StrokeThickness,
            FillColor = ColorToHex(polygon.FillColor),
            ZIndex = polygon.ZIndex,
            GeometryData = JsonSerializer.Serialize(geometryData)
        };
    }

    public ShapeModel ToModel(Shape entity)
    {
        if (entity.Type != ShapeType.Polygon)
            throw new ArgumentException("Entity must be Polygon type");

        var geometryData = JsonSerializer.Deserialize<PolygonGeometryData>(entity.GeometryData);
        if (geometryData == null || geometryData.Points == null)
            throw new InvalidOperationException("Failed to deserialize polygon geometry data");

        return new PolygonModel
        {
            Points = geometryData.Points.Select(p => new Point(p.X, p.Y)).ToList(),
            StrokeColor = ParseColor(entity.StrokeColor),
            StrokeThickness = entity.StrokeThickness,
            FillColor = ParseColor(entity.FillColor),
            ZIndex = entity.ZIndex
        };
    }

    private sealed class PolygonGeometryData
    {
        public List<PointData>? Points { get; set; }
    }

    private sealed class PointData
    {
        public double X { get; set; }
        public double Y { get; set; }
    }
}
