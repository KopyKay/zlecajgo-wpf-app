using System.Globalization;
using System.Windows;
using System.Windows.Data;
using ZlecajGoApi;

namespace ZlecajGoWpfApp.Converters;

public class MessageAlignmentConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string senderId)
        {
            return HorizontalAlignment.Left;
        }
        
        var currentUserId = UserSession.Instance.CurrentUser.Id;
        return senderId == currentUserId 
            ? HorizontalAlignment.Right 
            : HorizontalAlignment.Left;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}