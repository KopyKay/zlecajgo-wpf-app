using System.Windows;
using ZlecajGoWpfApp.ViewModels;

namespace ZlecajGoWpfApp.Views;

public partial class CalendarWindow : Window
{
    public CalendarWindow(CalendarViewModel viewModel)
    {
        InitializeComponent();
        
        DataContext = viewModel;
        
        CalendarViewModel.CalendarWindow = this;
        
        Loaded += async (s, e) =>
        {
            await viewModel.Initialize();
        };
    }
}