using System;
using System.Collections.Generic;
using PaintingApp.Contracts;
using PaintingApp.Services.Factories;

namespace PaintingApp.Services;

public class ShapeFactoryProvider
{
    private readonly Dictionary<string, IShapeFactory> _factories;

    public ShapeFactoryProvider()
    {
        _factories = new Dictionary<string, IShapeFactory>(StringComparer.OrdinalIgnoreCase)
        {
            { "Line", new LineFactory() },
            { "Rectangle", new RectangleFactory() },
            { "Circle", new CircleFactory() },
            { "Oval", new OvalFactory() },
            { "Triangle", new TriangleFactory() },
            { "Polygon", new PolygonFactory() }
        };
    }

    public IShapeFactory? GetFactory(string toolName)
    {
        if (_factories.TryGetValue(toolName, out var factory))
        {
            return factory;
        }

        return null;
    }

    public bool HasFactory(string toolName)
    {
        return _factories.ContainsKey(toolName);
    }

    public void RegisterFactory(string toolName, IShapeFactory factory)
    {
        _factories[toolName] = factory;
    }
}
