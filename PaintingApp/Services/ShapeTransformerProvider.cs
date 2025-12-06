using System;
using System.Collections.Generic;
using PaintingApp.Contracts;
using PaintingApp.Data.Entities;
using PaintingApp.Services.Transformers;

namespace PaintingApp.Services;

public class ShapeTransformerProvider
{
    private readonly Dictionary<ShapeType, IShapeTransformer> _transformers;

    public ShapeTransformerProvider()
    {
        _transformers = new Dictionary<ShapeType, IShapeTransformer>
        {
            { ShapeType.Line, new LineTransformer() },
            { ShapeType.Rectangle, new RectangleTransformer() },
            { ShapeType.Circle, new CircleTransformer() },
            { ShapeType.Oval, new OvalTransformer() },
            { ShapeType.Triangle, new TriangleTransformer() },
            { ShapeType.Polygon, new PolygonTransformer() }
        };
    }

    public IShapeTransformer GetTransformer(ShapeType type)
    {
        if (_transformers.TryGetValue(type, out var transformer))
        {
            return transformer;
        }

        throw new NotSupportedException($"Shape type {type} is not supported");
    }

    public void RegisterTransformer(ShapeType type, IShapeTransformer transformer)
    {
        _transformers[type] = transformer;
    }
}
