using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace PaintingApp.Helpers;

public class CountToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is int count && parameter is string threshold)
        {
            if (int.TryParse(threshold, out int thresholdValue))
            {
                return count == thresholdValue ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
