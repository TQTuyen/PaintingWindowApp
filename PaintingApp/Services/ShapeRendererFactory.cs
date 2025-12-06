using System;
using System.Collections.Generic;
using PaintingApp.Contracts;
using PaintingApp.Data.Entities;
using PaintingApp.Services.Renderers;

namespace PaintingApp.Services;

public class ShapeRendererFactory
{
    private readonly Dictionary<ShapeType, IShapeRenderer> _renderers;

    public ShapeRendererFactory(IStrokeDashProvider dashProvider)
    {
        _renderers = new Dictionary<ShapeType, IShapeRenderer>
        {
            { ShapeType.Line, new LineRenderer(dashProvider) },
            { ShapeType.Rectangle, new RectangleRenderer(dashProvider) },
            { ShapeType.Circle, new CircleRenderer(dashProvider) },
            { ShapeType.Oval, new OvalRenderer(dashProvider) },
            { ShapeType.Triangle, new PolygonRenderer(dashProvider) },
            { ShapeType.Polygon, new PolygonRenderer(dashProvider) }
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
