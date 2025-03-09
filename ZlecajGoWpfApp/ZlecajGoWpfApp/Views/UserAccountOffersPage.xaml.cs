using System.Windows;
using System.Windows.Controls;
using ZlecajGoWpfApp.ViewModels;

namespace ZlecajGoWpfApp.Views;

public partial class UserAccountOffersPage : Page
{
    public UserAccountOffersPage(UserAccountOffersViewModel viewModel)
    {
        InitializeComponent();

        DataContext = viewModel;
        
        Loaded += OnLoaded;
        Loaded += async (s, e) => await viewModel.GetUserOffersAsync();
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        var window = Window.GetWindow(this) as UserAccountMenuContextWindow;
        UserAccountOffersViewModel.ParentWindow = window;
        UserAccountOffersViewModel.ParentWindowFrame = window!.MainFrame;
    }
}