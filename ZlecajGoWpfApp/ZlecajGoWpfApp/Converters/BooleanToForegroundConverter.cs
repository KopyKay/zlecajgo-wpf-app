using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ZlecajGoWpfApp.Converters;

public class BooleanToForegroundConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return (bool)value!
            ? Brushes.Black
            : Brushes.Gray;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}