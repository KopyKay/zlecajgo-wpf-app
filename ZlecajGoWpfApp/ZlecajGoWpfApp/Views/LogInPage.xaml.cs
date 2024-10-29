using System.Windows.Controls;
using ZlecajGoWpfApp.ViewModels;

namespace ZlecajGoWpfApp.Views;

public partial class LogInPage : Page
{
    public LogInPage(LogInViewModel viewModel)
    {
        InitializeComponent();
        
        DataContext = viewModel;
    }
}