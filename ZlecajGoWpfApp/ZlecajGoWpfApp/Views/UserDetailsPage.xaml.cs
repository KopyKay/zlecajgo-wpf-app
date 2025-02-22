using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ZlecajGoWpfApp.ViewModels;

namespace ZlecajGoWpfApp.Views;

public partial class UserDetailsPage : Page
{
    public UserDetailsPage(UserAccountMenuViewModel viewModel)
    {
        InitializeComponent();
        
        DataContext = viewModel;
    }
    
    private const string PhoneNumberPolandPrefix = "+48";
    
    private void TextBox_OnPasting(object sender, DataObjectPastingEventArgs e)
    {
        e.CancelCommand();
    }
    
    private void NewPhoneNumberTextBox_OnPreviewKeyDown(object sender, KeyEventArgs e)
    {
        var textBox = (TextBox)sender;
        
        switch (e.Key)
        {
            case Key.Space:
            case Key.Clear:
            case Key.Back when textBox.CaretIndex <= PhoneNumberPolandPrefix.Length:
            case Key.Delete when textBox.SelectionStart < PhoneNumberPolandPrefix.Length:
                e.Handled = true;
                break;
        }
    }
    
    private void NewPhoneNumberTextBox_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        var textBox = (TextBox)sender;
        var newText = textBox.Text.Insert(textBox.CaretIndex, e.Text);
        
        if (!char.IsDigit(e.Text, 0) ||
            textBox.CaretIndex < PhoneNumberPolandPrefix.Length ||
            newText.Length > 12)
        {
            e.Handled = true;
        }
    }
    
    private void NewPhoneNumberTextBox_OnGotFocus(object sender, RoutedEventArgs e)
    {
        var textBox = (TextBox)sender;
        
        if (!string.IsNullOrWhiteSpace(textBox.Text))
        {
            return;
        }
        
        textBox.Text = PhoneNumberPolandPrefix;
        textBox.CaretIndex = textBox.Text.Length;
    }

    private void NewPhoneNumberTextBox_OnLostFocus(object sender, RoutedEventArgs e)
    {
        var textBox = (TextBox)sender;

        if (textBox.Text != PhoneNumberPolandPrefix)
        {
            return;
        }
        
        textBox.Text = string.Empty;
    }
}