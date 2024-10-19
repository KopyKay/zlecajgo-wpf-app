using System.Windows.Controls;

namespace ZlecajGoWpfApp.Services;

public class NavigationService(MainWindow mainWindow)
{
    public void NavigateTo(Page page)
    {
        mainWindow.MainFrame.Content = page;
    }
}