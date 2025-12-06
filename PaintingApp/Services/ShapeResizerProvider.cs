using System;
using System.Collections.Generic;
using PaintingApp.Contracts;
using PaintingApp.Data.Entities;
using PaintingApp.Services.Resizers;

namespace PaintingApp.Services;

public class ShapeResizerProvider
{
    private readonly Dictionary<ShapeType, IShapeResizer> _resizers;

    public ShapeResizerProvider()
    {
        _resizers = new Dictionary<ShapeType, IShapeResizer>
        {
            { ShapeType.Line, new LineResizer() },
            { ShapeType.Rectangle, new RectangleResizer() },
            { ShapeType.Circle, new CircleResizer() },
            { ShapeType.Oval, new OvalResizer() },
            { ShapeType.Triangle, new TriangleResizer() },
            { ShapeType.Polygon, new PolygonResizer() }
        };
    }

    public IShapeResizer GetResizer(ShapeType type)
    {
        if (_resizers.TryGetValue(type, out var resizer))
        {
            return resizer;
        }

        throw new NotSupportedException($"Shape type {type} is not supported");
    }
}
