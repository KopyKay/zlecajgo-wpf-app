using System.Windows.Controls;
using ZlecajGoWpfApp.ViewModel;

namespace ZlecajGoWpfApp.View;

public partial class SignUpPage : Page
{
    public SignUpPage(SignUpViewModel viewModel)
    {
        InitializeComponent();
        
        DataContext = viewModel;
    }
}