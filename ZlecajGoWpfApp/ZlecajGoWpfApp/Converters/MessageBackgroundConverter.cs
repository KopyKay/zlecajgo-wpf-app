using System.Drawing;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using ZlecajGoApi;

namespace ZlecajGoWpfApp.Converters;

public class MessageBackgroundConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string senderId)
        {
            return Application.Current.Resources["AccentColorBrush2"]!;
        }
        
        var currentUserId = UserSession.Instance.CurrentUser.Id;
        return senderId == currentUserId 
            ? Application.Current.Resources["AccentColorBrush1"]! 
            : Application.Current.Resources["AccentColorBrush2"]!;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}