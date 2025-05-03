using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ZlecajGoWpfApp.Converters;

public class BooleanToBackgroundConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var boolValue = (bool)value!;
        
        if (boolValue)
            return Brushes.White;
        return parameter != null
            ? new SolidColorBrush((Color)ColorConverter.ConvertFromString(parameter.ToString())) 
            : Brushes.WhiteSmoke;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}