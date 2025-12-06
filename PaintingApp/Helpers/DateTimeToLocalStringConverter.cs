using Microsoft.UI.Xaml.Data;
using System;

namespace PaintingApp.Helpers;

public class DateTimeToLocalStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is DateTime dateTime)
        {
            // SQLite doesn't preserve DateTimeKind, so Unspecified times from DB are actually UTC
            var localTime = dateTime.Kind == DateTimeKind.Local 
                ? dateTime 
                : DateTime.SpecifyKind(dateTime, DateTimeKind.Utc).ToLocalTime();
            return localTime.ToString("dd/MM/yy HH:mm:ss");
        }
        return string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
