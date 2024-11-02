using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace ZlecajGoWpfApp.Services;

public class NavigationService(IServiceProvider serviceProvider, MainWindow mainWindow)
{
    public void NavigateTo<T>() where T : Page
    {
        var page = serviceProvider.GetRequiredService<T>();
        mainWindow.MainFrame.Content = page;
    }
}