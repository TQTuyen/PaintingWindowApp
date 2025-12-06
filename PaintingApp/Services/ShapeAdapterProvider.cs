using System;
using System.Collections.Generic;
using PaintingApp.Contracts;
using PaintingApp.Data.Entities;
using PaintingApp.Services.Adapters;

namespace PaintingApp.Services;

public class ShapeAdapterProvider
{
    private readonly Dictionary<ShapeType, IShapeAdapter> _adapters;

    public ShapeAdapterProvider()
    {
        _adapters = new Dictionary<ShapeType, IShapeAdapter>
        {
            { ShapeType.Line, new LineAdapter() },
            { ShapeType.Rectangle, new RectangleAdapter() },
            { ShapeType.Circle, new CircleAdapter() },
            { ShapeType.Oval, new OvalAdapter() },
            { ShapeType.Triangle, new TriangleAdapter() },
            { ShapeType.Polygon, new PolygonAdapter() }
        };
    }

    public IShapeAdapter GetAdapter(ShapeType type)
    {
        if (_adapters.TryGetValue(type, out var adapter))
        {
            return adapter;
        }

        throw new NotSupportedException($"Shape type {type} is not supported");
    }

    public void RegisterAdapter(ShapeType type, IShapeAdapter adapter)
    {
        _adapters[type] = adapter;
    }
}
