using System.Windows;
using System.Windows.Controls;

namespace ZlecajGoWpfApp.Services.Navigation;

public interface INavigationService
{
    void NavigateTo<T>(Window? window = null, string? frameName = null) where T : Page;
}