using System.Globalization;
using System.Windows.Data;

namespace ZlecajGoWpfApp.Converters;

public class DurationInDaysConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int days)
        {
            return days switch
            {
                1 => $"{days} dzień",
                _ => $"{days} dni"
            };
        }
        return value;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string str)
        {
            return value;
        }

        return str switch
        {
            _ when str.EndsWith("dzień") => int.Parse(str.Replace(" dzień", string.Empty)),
            _ when str.EndsWith("dni") => int.Parse(str.Replace(" dni", string.Empty)),
            _ => value
        };
    }
}