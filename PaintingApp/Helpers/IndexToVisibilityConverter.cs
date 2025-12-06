using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;

namespace PaintingApp.Helpers;

public class IndexToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is int index && parameter is string targetIndex)
        {
            return index == int.Parse(targetIndex) ? Visibility.Visible : Visibility.Collapsed;
        }
        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
