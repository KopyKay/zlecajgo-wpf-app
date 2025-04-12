using System.Globalization;
using System.Windows.Data;
using MaterialDesignThemes.Wpf;

namespace ZlecajGoWpfApp.Converters;

public class MessageReadIconKindConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not bool isRead)
        {
            return PackIconKind.CheckboxMultipleMarkedCircle;
        }
        
        return isRead 
            ? PackIconKind.CheckboxMarkedCircle 
            : PackIconKind.CheckboxMarkedCircleOutline ;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}