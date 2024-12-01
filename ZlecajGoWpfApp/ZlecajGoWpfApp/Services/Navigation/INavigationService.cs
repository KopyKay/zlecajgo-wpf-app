using System.Windows.Controls;

namespace ZlecajGoWpfApp.Services.Navigation;

public interface INavigationService
{
    void NavigateTo<T>() where T : Page;
}