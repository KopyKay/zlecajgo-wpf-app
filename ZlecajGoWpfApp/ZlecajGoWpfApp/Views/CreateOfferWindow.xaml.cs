using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ZlecajGoWpfApp.Helpers;
using ZlecajGoWpfApp.ViewModels;

namespace ZlecajGoWpfApp.Views;

public partial class CreateOfferWindow : Window
{
    public CreateOfferWindow(CreateOfferViewModel viewModel)
    {
        InitializeComponent();
        
        DataContext = viewModel;
        
        SubscribeToViewModelEvents(viewModel);
        
        Loaded += async (s, e) => await viewModel.LoadPostalAddressesAsync();
    }

    private void SubscribeToViewModelEvents(CreateOfferViewModel viewModel)
    {
        viewModel.RequestWindowClose += (s, e) => Window.GetWindow(this)?.Close();
    }
    
    private void HandleTextInput(object sender, TextCompositionEventArgs e, Regex regex, int firstInsertPosition, int secondInsertPosition, string insertChar)
    {
        var textBox = (TextBox)sender;
        var newText = textBox.Text + e.Text;
        
        switch (newText.Length)
        {
            case var len when len == firstInsertPosition && !newText.Contains(insertChar):
                textBox.Text = newText + insertChar;
                textBox.CaretIndex = textBox.Text.Length;
                e.Handled = true;
                break;
            case var len when len == secondInsertPosition && !newText.Contains(insertChar) && char.IsDigit(e.Text, 0):
                textBox.Text = newText.Insert(firstInsertPosition, insertChar);
                textBox.CaretIndex = textBox.Text.Length;
                e.Handled = true;
                break;
            default:
                e.Handled = !regex.IsMatch(newText);
                break;
        }
    }
    
    private void TextBox_OnPasting(object sender, DataObjectPastingEventArgs e)
    {
        e.CancelCommand();
    }

    private void TextBox_OnPreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Space)
        {
            e.Handled = true;
        }
    }
    
    private void PostalCodeTextBox_OnLostFocus(object sender, RoutedEventArgs e)
    {
        var textBox = (TextBox)sender;
        var text = textBox.Text;

        if (text.Length == 5 && !text.Contains('-'))
        {
            textBox.Text = text.Insert(2, "-");
        }
    }

    private void PostalCodeTextBox_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        var textBox = (TextBox)sender;
        var newText = textBox.Text + e.Text;

        if ((newText.Length == 1 && e.Text == "-") || (newText.Length == 2 && e.Text == "-"))
        {
            e.Handled = true;
            return;
        }
        
        HandleTextInput(sender, e, ValidationHelper.ValidPostalCodePreviewInputRegex(), 2, 3, "-");
    }
    
    private void PriceTextBox_LostFocus(object sender, RoutedEventArgs e)
    {
        var textBox = (TextBox)sender;
        var text = textBox.Text;

        if (text.Contains(','))
        {
            var parts = text.Split(',');
            textBox.Text = parts.Length switch
            {
                2 when parts[1].Length == 0 => parts[0] + ",00",
                2 when parts[1].Length == 1 => parts[0] + "," + parts[1] + "0",
                _ => textBox.Text
            };
        }
        else if (text.Length > 0)
        {
            textBox.Text = text + ",00";
        }
    }
    
    private void PriceTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        HandleTextInput(sender, e, ValidationHelper.ValidPricePreviewInputRegex(), 7, 8, ",");
    }
}