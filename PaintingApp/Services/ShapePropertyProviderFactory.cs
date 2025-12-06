using PaintingApp.Contracts;
using PaintingApp.Models;
using PaintingApp.Services.PropertyProviders;
using PaintingApp.Data.Entities;
using System;
using System.Collections.Generic;

namespace PaintingApp.Services
{
    public class ShapePropertyProviderFactory
    {
        private readonly Dictionary<ShapeType, IShapePropertyProvider> _providers;

        public ShapePropertyProviderFactory()
        {
            _providers = new Dictionary<ShapeType, IShapePropertyProvider>
            {
                { ShapeType.Line, new LinePropertyProvider() },
                { ShapeType.Rectangle, new RectanglePropertyProvider() },
                { ShapeType.Circle, new CirclePropertyProvider() },
                { ShapeType.Oval, new OvalPropertyProvider() },
                { ShapeType.Triangle, new TrianglePropertyProvider() },
                { ShapeType.Polygon, new PolygonPropertyProvider() }
            };
        }

        public IShapePropertyProvider GetProvider(ShapeType type)
        {
            if (_providers.TryGetValue(type, out var provider))
            {
                return provider;
            }

            throw new NotSupportedException($"Shape type {type} is not supported");
        }
    }
}
