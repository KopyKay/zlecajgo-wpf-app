using System.Windows.Controls;
using ZlecajGoWpfApp.ViewModels;

namespace ZlecajGoWpfApp.Views;

public partial class SetUpUserCredentialsPage : Page
{
    public SetUpUserCredentialsPage(SetUpUserCredentialsViewModel viewModel)
    {
        InitializeComponent();
        
        DataContext = viewModel;
    }
}