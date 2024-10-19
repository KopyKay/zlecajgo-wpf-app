using System.Windows.Controls;
using ZlecajGoWpfApp.ViewModel;

namespace ZlecajGoWpfApp.View;

public partial class SetUpUserCredentialsPage : Page
{
    public SetUpUserCredentialsPage(SetUpUserCredentialsViewModel viewModel)
    {
        InitializeComponent();
        
        DataContext = viewModel;
    }
}