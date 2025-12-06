using System;
using System.Text.Json;
using PaintingApp.Contracts;
using PaintingApp.Data.Entities;
using PaintingApp.Models;
using Windows.Foundation;

namespace PaintingApp.Services.Adapters;

public class OvalAdapter : BaseShapeAdapter, IShapeAdapter
{
    public Shape ToEntity(ShapeModel model)
    {
        if (model is not OvalModel oval)
            throw new ArgumentException("Model must be OvalModel");

        var geometryData = new
        {
            CenterX = oval.Center.X,
            CenterY = oval.Center.Y,
            oval.RadiusX,
            oval.RadiusY
        };

        return new Shape
        {
            Type = ShapeType.Oval,
            StrokeColor = ColorToHex(oval.StrokeColor),
            StrokeThickness = oval.StrokeThickness,
            StrokeStyle = oval.StrokeDashStyle.ToString(),
            FillColor = ColorToHex(oval.FillColor),
            ZIndex = oval.ZIndex,
            GeometryData = JsonSerializer.Serialize(geometryData)
        };
    }

    public ShapeModel ToModel(Shape entity)
    {
        if (entity.Type != ShapeType.Oval)
            throw new ArgumentException("Entity must be Oval type");

        var geometryData = JsonSerializer.Deserialize<OvalGeometryData>(entity.GeometryData);
        if (geometryData == null)
            throw new InvalidOperationException("Failed to deserialize oval geometry data");

        return new OvalModel
        {
            Center = new Point(geometryData.CenterX, geometryData.CenterY),
            RadiusX = geometryData.RadiusX,
            RadiusY = geometryData.RadiusY,
            StrokeColor = ParseColor(entity.StrokeColor),
            StrokeThickness = entity.StrokeThickness,
            StrokeDashStyle = ParseStrokeDashStyle(entity.StrokeStyle),
            FillColor = ParseColor(entity.FillColor),
            ZIndex = entity.ZIndex
        };
    }

    private sealed class OvalGeometryData
    {
        public double CenterX { get; set; }
        public double CenterY { get; set; }
        public double RadiusX { get; set; }
        public double RadiusY { get; set; }
    }
}
