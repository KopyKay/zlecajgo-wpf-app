using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace ZlecajGoWpfApp.Services.Navigation;

public class NavigationService(IServiceProvider serviceProvider, MainWindow mainWindow) : INavigationService
{
    public void NavigateTo<T>() where T : Page
    {
        var page = serviceProvider.GetRequiredService<T>();
        mainWindow.MainFrame.Content = page;
    }
}