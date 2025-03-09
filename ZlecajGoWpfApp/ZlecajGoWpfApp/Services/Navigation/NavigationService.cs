using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace ZlecajGoWpfApp.Services.Navigation;

public class NavigationService(IServiceProvider serviceProvider, MainWindow mainWindow) : INavigationService
{
    public void NavigateTo<T>(Window? window = null, Frame? frame = null) where T : Page
    {
        var page = serviceProvider.GetRequiredService<T>();

        if (window is not null && frame is null)
        {
            window.Content = page;
            return;
        }

        if (window is not null && frame is not null)
        {
            if (window.FindName(frame.Name) is Frame frameObj)
            {
                frameObj.Content = page;
            }
            else
            {
                throw new InvalidOperationException($"A Frame with the name '{frame}' was not found in the specified window.");
            }
            return;
        }
        
        mainWindow.MainFrame.Content = page;
    }
}