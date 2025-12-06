using Microsoft.UI.Xaml.Data;
using System;

namespace PaintingApp.Helpers;

public class PercentageToWidthConverter : IValueConverter
{
    private const double MaxWidth = 300;

    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is double percentage)
        {
            var width = (percentage / 100.0) * MaxWidth;
            return Math.Max(width, 4);
        }
        return 4.0;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
