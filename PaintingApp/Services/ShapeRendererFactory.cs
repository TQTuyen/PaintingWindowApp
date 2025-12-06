using System;
using System.Collections.Generic;
using PaintingApp.Contracts;
using PaintingApp.Data.Entities;
using PaintingApp.Services.Renderers;

namespace PaintingApp.Services;

public class ShapeRendererFactory
{
    private readonly Dictionary<ShapeType, IShapeRenderer> _renderers;

    public ShapeRendererFactory()
    {
        _renderers = new Dictionary<ShapeType, IShapeRenderer>
        {
            { ShapeType.Line, new LineRenderer() },
            { ShapeType.Rectangle, new RectangleRenderer() },
            { ShapeType.Circle, new CircleRenderer() },
            { ShapeType.Oval, new OvalRenderer() },
            { ShapeType.Triangle, new PolygonRenderer() },
            { ShapeType.Polygon, new PolygonRenderer() }
        };
    }

    public IShapeRenderer GetRenderer(ShapeType type)
    {
        if (_renderers.TryGetValue(type, out var renderer))
        {
            return renderer;
        }

        throw new NotSupportedException($"Shape type {type} is not supported");
    }

    public void RegisterRenderer(ShapeType type, IShapeRenderer renderer)
    {
        _renderers[type] = renderer;
    }
}
