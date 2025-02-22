using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace ZlecajGoWpfApp.Services.Navigation;

public class NavigationService(IServiceProvider serviceProvider, MainWindow mainWindow) : INavigationService
{
    public void NavigateTo<T>(Window? window = null, string? frameName = null) where T : Page
    {
        var page = serviceProvider.GetRequiredService<T>();

        if (window is not null && frameName is null)
        {
            window.Content = page;
            return;
        }

        if (window is not null && frameName is not null)
        {
            if (window.FindName(frameName) is Frame frame)
            {
                frame.Content = page;
            }
            else
            {
                throw new InvalidOperationException($"A Frame with the name '{frameName}' was not found in the specified window.");
            }
            return;
        }
        
        mainWindow.MainFrame.Content = page;
    }
}