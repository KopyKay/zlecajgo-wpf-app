using System.Windows;
using ZlecajGoWpfApp.ViewModels;

namespace ZlecajGoWpfApp.Views;

public partial class UserAccountMenuContextWindow : Window
{
    public UserAccountMenuContextWindow(UserAccountMenuNavigationViewModel viewModel)
    {
        InitializeComponent();

        SizeChanged += UserAccountMenuContextWindow_OnSizeChanged;
        
        DataContext = viewModel;
        UserAccountMenuNavigationViewModel.UserAccountMenuContextWindow = this;
        UserAccountMenuNavigationViewModel.UserAccountMenuContextWindowFrame = this.MainFrame;
        viewModel.NavigateToUserAccountMenuNavigationCommand.Execute(this);
    }

    private void UserAccountMenuContextWindow_OnSizeChanged(object? sender, EventArgs e)
    {
        CenterWindowOnScreen();
    }
    
    private void CenterWindowOnScreen()
    {
        var screenWidth = SystemParameters.PrimaryScreenWidth;
        var screenHeight = SystemParameters.PrimaryScreenHeight;
        var windowWidth = this.ActualWidth;
        var windowHeight = this.ActualHeight;
        this.Left = (screenWidth / 2) - (windowWidth / 2);
        this.Top = (screenHeight / 2) - (windowHeight / 2);
    }
}