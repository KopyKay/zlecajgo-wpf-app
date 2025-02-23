using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ZlecajGoWpfApp.Constants;
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
    {
        e.CancelCommand();
    }
    
    private void PhoneTextBox_OnPreviewKeyDown(object sender, KeyEventArgs e)
    {
        var textBox = (TextBox)sender;
        
        switch (e.Key)
        {
            case Key.Space:
            case Key.Clear:
            case Key.Back when textBox.CaretIndex <= CountryCode.Poland.Length:
            case Key.Delete when textBox.SelectionStart < CountryCode.Poland.Length:
                e.Handled = true;
                break;
        }
    }
    
    private void PhoneNumberTextBox_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        var textBox = (TextBox)sender;
        var newText = textBox.Text.Insert(textBox.CaretIndex, e.Text);
        
        if (!char.IsDigit(e.Text, 0) ||
            textBox.CaretIndex < CountryCode.Poland.Length ||
            newText.Length > 12)
        {
            e.Handled = true;
        }
    }
    
    private void PhoneNumberTextBox_OnGotFocus(object sender, RoutedEventArgs e)
    {
        var textBox = (TextBox)sender;
        
        if (!string.IsNullOrWhiteSpace(textBox.Text))
        {
            return;
        }
        
        textBox.Text = CountryCode.Poland;
        textBox.CaretIndex = textBox.Text.Length;
    }

    private void PhoneNumberTextBox_OnLostFocus(object sender, RoutedEventArgs e)
    {
        var textBox = (TextBox)sender;

        if (textBox.Text != CountryCode.Poland)
        {
            return;
        }
        
        textBox.Text = string.Empty;
    }
}