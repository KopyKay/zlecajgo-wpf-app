using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ZlecajGoWpfApp.Helpers;
using ZlecajGoWpfApp.ViewModels;

namespace ZlecajGoWpfApp.Views;

public partial class UserAccountDetailsPage : Page
{
    public UserAccountDetailsPage(UserAccountDetailsViewModel viewModel)
    {
        InitializeComponent();
        
        DataContext = viewModel;

        Loaded += OnLoaded;
    }
    
    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        var window = Window.GetWindow(this) as UserAccountMenuContextWindow;
        UserAccountDetailsViewModel.ParentWindow = window;
        UserAccountDetailsViewModel.ParentWindowFrame = window!.MainFrame;
    }
    
    private void TextBox_OnPasting(object sender, DataObjectPastingEventArgs e) 
        => e.CancelCommand();
    
    private void NewPhoneNumberTextBox_OnPreviewKeyDown(object sender, KeyEventArgs e)
        => PhoneNumberTextBoxHelper.OnPreviewKeyDown((TextBox)sender, e);
    
    private void NewPhoneNumberTextBox_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        => PhoneNumberTextBoxHelper.OnPreviewTextInput((TextBox)sender, e);
    
    private void NewPhoneNumberTextBox_OnGotFocus(object sender, RoutedEventArgs e)
        => PhoneNumberTextBoxHelper.OnGotFocus((TextBox)sender, e);

    private void NewPhoneNumberTextBox_OnLostFocus(object sender, RoutedEventArgs e)
        => PhoneNumberTextBoxHelper.OnLostFocus((TextBox)sender, e);
}