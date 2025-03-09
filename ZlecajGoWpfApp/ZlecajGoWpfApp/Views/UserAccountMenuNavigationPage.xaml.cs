using System.Windows.Controls;
using ZlecajGoWpfApp.ViewModels;

namespace ZlecajGoWpfApp.Views;

public partial class UserAccountMenuNavigationPage : Page
{
    public UserAccountMenuNavigationPage(UserAccountMenuNavigationViewModel viewModel)
    {
        InitializeComponent();
        
        DataContext = viewModel;
    }
}