using Microsoft.UI.Xaml.Data;
using System;

namespace PaintingApp.Helpers;

public class DoubleToPercentageConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is double percentage)
        {
            return $"{percentage:F1}%";
        }
        return "0%";
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
