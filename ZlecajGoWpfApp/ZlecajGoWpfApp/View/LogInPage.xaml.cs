using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using ZlecajGoWpfApp.ViewModel;

namespace ZlecajGoWpfApp.View;

public partial class LogInPage : Page
{
    public LogInPage(LogInViewModel viewModel)
    {
        InitializeComponent();
        
        DataContext = viewModel;
    }
}