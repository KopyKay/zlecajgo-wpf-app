using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using ZlecajGoWpfApp.Enums;

namespace ZlecajGoWpfApp.Converters;

public class OfferTypeToBrushConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int typeId)
        {
            return (typeId switch
            {
                (int)OfferType.Request => Application.Current.Resources["AccentColorBrush1"],
                (int)OfferType.Service => Application.Current.Resources["AccentColorBrush2"],
                _ => new SolidColorBrush(Colors.SaddleBrown)
            })!;
        }
        
        return Application.Current.Resources["AccentColorsGradientBrush"]!;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}