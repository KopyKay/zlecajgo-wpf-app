using System.Globalization;
using System.Windows.Data;
using ZlecajGoWpfApp.Enums;

namespace ZlecajGoWpfApp.Converters;

public class OfferStatusToEnumConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return (OfferStatus)value!;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return (int)(OfferStatus)value!;
    }
}