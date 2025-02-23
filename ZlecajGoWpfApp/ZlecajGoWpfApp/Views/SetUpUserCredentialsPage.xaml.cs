using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ZlecajGoWpfApp.Helpers;
using ZlecajGoWpfApp.ViewModels;

namespace ZlecajGoWpfApp.Views;

public partial class SetUpUserCredentialsPage : Page
{
    public SetUpUserCredentialsPage(SetUpUserCredentialsViewModel viewModel)
    {
        InitializeComponent();
        
        DataContext = viewModel;
    }
    
    private void TextBox_OnPasting(object sender, DataObjectPastingEventArgs e) 
        => e.CancelCommand();
    
    private void PhoneTextBox_OnPreviewKeyDown(object sender, KeyEventArgs e)
        => PhoneNumberTextBoxHelper.OnPreviewKeyDown((TextBox)sender, e);
    
    private void PhoneNumberTextBox_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        => PhoneNumberTextBoxHelper.OnPreviewTextInput((TextBox)sender, e);
    
    private void PhoneNumberTextBox_OnGotFocus(object sender, RoutedEventArgs e)
        => PhoneNumberTextBoxHelper.OnGotFocus((TextBox)sender, e);

    private void PhoneNumberTextBox_OnLostFocus(object sender, RoutedEventArgs e)
        => PhoneNumberTextBoxHelper.OnLostFocus((TextBox)sender, e);
}