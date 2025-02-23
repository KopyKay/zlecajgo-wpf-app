using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ZlecajGoWpfApp.Constants;

namespace ZlecajGoWpfApp.Helpers;

public static class PhoneNumberTextBoxHelper
{
    public static void OnPreviewKeyDown(TextBox textBox, KeyEventArgs e)
    {
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

    public static void OnPreviewTextInput(TextBox textBox, TextCompositionEventArgs e)
    {
        var newText = textBox.Text.Insert(textBox.CaretIndex, e.Text);

        if (!char.IsDigit(e.Text, 0) || textBox.CaretIndex < CountryCode.Poland.Length || newText.Length > 12)
        {
            e.Handled = true;
        }
    }

    public static void OnGotFocus(TextBox textBox, RoutedEventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(textBox.Text)) return;

        textBox.Text = CountryCode.Poland;
        textBox.CaretIndex = textBox.Text.Length;
    }
    
    public static void OnLostFocus(TextBox textBox, RoutedEventArgs e)
    {
        if (textBox.Text != CountryCode.Poland) return;

        textBox.Text = string.Empty;
    }
}