using System;
using System.Text.Json;
using PaintingApp.Contracts;
using PaintingApp.Data.Entities;
using PaintingApp.Models;
using Windows.Foundation;

namespace PaintingApp.Services.Adapters;

public class LineAdapter : BaseShapeAdapter, IShapeAdapter
{
    public Shape ToEntity(ShapeModel model)
    {
        if (model is not LineModel line)
            throw new ArgumentException("Model must be LineModel");

        var geometryData = new
        {
            StartX = line.StartPoint.X,
            StartY = line.StartPoint.Y,
            EndX = line.EndPoint.X,
            EndY = line.EndPoint.Y
        };

        return new Shape
        {
            Type = ShapeType.Line,
            StrokeColor = ColorToHex(line.StrokeColor),
            StrokeThickness = line.StrokeThickness,
            FillColor = ColorToHex(line.FillColor),
            ZIndex = line.ZIndex,
            GeometryData = JsonSerializer.Serialize(geometryData)
        };
    }

    public ShapeModel ToModel(Shape entity)
    {
        if (entity.Type != ShapeType.Line)
            throw new ArgumentException("Entity must be Line type");

        var geometryData = JsonSerializer.Deserialize<LineGeometryData>(entity.GeometryData);
        if (geometryData == null)
            throw new InvalidOperationException("Failed to deserialize line geometry data");

        return new LineModel
        {
            StartPoint = new Point(geometryData.StartX, geometryData.StartY),
            EndPoint = new Point(geometryData.EndX, geometryData.EndY),
            StrokeColor = ParseColor(entity.StrokeColor),
            StrokeThickness = entity.StrokeThickness,
            FillColor = ParseColor(entity.FillColor),
            ZIndex = entity.ZIndex
        };
    }

    private sealed class LineGeometryData
    {
        public double StartX { get; set; }
        public double StartY { get; set; }
        public double EndX { get; set; }
        public double EndY { get; set; }
    }
}
