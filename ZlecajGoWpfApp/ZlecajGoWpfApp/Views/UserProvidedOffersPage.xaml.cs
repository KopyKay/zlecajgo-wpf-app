using System.Windows.Controls;
using ZlecajGoWpfApp.ViewModels;

namespace ZlecajGoWpfApp.Views;

public partial class UserProvidedOffersPage : Page
{
    public UserProvidedOffersPage(UserAccountMenuViewModel viewModel)
    {
        InitializeComponent();

        DataContext = viewModel;
    }
}