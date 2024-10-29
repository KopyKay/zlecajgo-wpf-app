using System.Windows.Controls;
using ZlecajGoWpfApp.ViewModels;

namespace ZlecajGoWpfApp.Views;

public partial class SignUpPage : Page
{
    public SignUpPage(SignUpViewModel viewModel)
    {
        InitializeComponent();
        
        DataContext = viewModel;
    }
}