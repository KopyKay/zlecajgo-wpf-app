using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using ZlecajGoApi.Dtos;
using ZlecajGoWpfApp.Helpers;

namespace ZlecajGoWpfApp.CustomControls;

public partial class EditOfferDialog : BaseDialog, INotifyPropertyChanged
{
    public EditOfferDialog(OfferDto offerDto)
    {
        InitializeComponent();
        DataContext = offerDto;
        CalculateDurationInDays(offerDto);

        _newOfferPrice = offerDto.Price.ToString(new CultureInfo("pl-PL"));
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    
    private int _selectedDuration;
    public int SelectedDuration
    {
        get => _selectedDuration;
        set
        {
            if (_selectedDuration == value) return;
            _selectedDuration = value;
            OnPropertyChanged();
        }
    }

    private string _newOfferPrice;
    public string NewOfferPrice
    {
        get => _newOfferPrice;
        set
        {
            if (_newOfferPrice == value) return;
            _newOfferPrice = value;
            OnPropertyChanged();
        }
    }
    
    private void OnPropertyChanged([CallerMemberName] string propertyName = null!)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    
    private void CalculateDurationInDays(OfferDto offerDto)
    {
        var remainingTime = offerDto.ExpiryDateTime - DateTime.Now;
        var remainingDays = (int)Math.Ceiling(remainingTime.TotalDays);
        
        if (remainingDays < 2)
        {
            DurationComboBox.IsEnabled = false;
        }
        else
        {
            var durationInDays = Enumerable.Range(1, remainingDays - 1).ToArray();
            DurationComboBox.ItemsSource = durationInDays;
        }
    }
    
    private void TextBox_OnPasting(object sender, DataObjectPastingEventArgs e)
    {
        e.CancelCommand();
    }
    
    private void PriceTextBox_OnPreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Space)
        {
            e.Handled = true;
        }
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
        const int firstInsertPosition = 7;
        const int secondInsertPosition = 8;
        const string insertChar = ",";
        var regex = ValidationHelper.ValidPricePreviewInputRegex();
        
        var textBox = (TextBox)sender;
        var newText = textBox.Text + e.Text;
        
        switch (newText.Length)
        {
            case firstInsertPosition when !newText.Contains(insertChar):
                textBox.Text = newText + insertChar;
                textBox.CaretIndex = textBox.Text.Length;
                e.Handled = true;
                break;
            case secondInsertPosition when !newText.Contains(insertChar) && char.IsDigit(e.Text, 0):
                textBox.Text = newText.Insert(firstInsertPosition, insertChar);
                textBox.CaretIndex = textBox.Text.Length;
                e.Handled = true;
                break;
            default:
                e.Handled = !regex.IsMatch(newText);
                break;
        }
    }
    
    private void CancelButton_OnClick(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    private void ConfirmButton_OnClick(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
        Close();
    }
}