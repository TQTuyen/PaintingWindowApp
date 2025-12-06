using System;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using PaintingApp.Models;

namespace PaintingApp.Helpers;

public class StrokeDashStyleToDoubleCollectionConverter : IValueConverter
{
    public object? Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is StrokeDashStyle style)
        {
            return style switch
            {
                StrokeDashStyle.Solid => null,
                StrokeDashStyle.Dash => new DoubleCollection { 4, 2 },
                StrokeDashStyle.Dot => new DoubleCollection { 1, 2 },
                StrokeDashStyle.DashDot => new DoubleCollection { 4, 2, 1, 2 },
                StrokeDashStyle.DashDotDot => new DoubleCollection { 4, 2, 1, 2, 1, 2 },
                _ => null
            };
        }
        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
