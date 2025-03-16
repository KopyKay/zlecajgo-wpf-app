using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using ZlecajGoApi;
using ZlecajGoWpfApp.Enums;

namespace ZlecajGoWpfApp.CustomControls;

public partial class ConfirmPasswordDialog : BaseDialog
{
    public ConfirmPasswordDialog(IApiClient apiClient)
    {
        InitializeComponent();
        
        _apiClient = apiClient;
    }
    
    private readonly IApiClient _apiClient;
    
    private string Password { get; set; } = string.Empty;
    public string EnteredPassword => Password;

    private void PasswordBox_OnPasswordChanged(object sender, RoutedEventArgs e)
    {
        if (sender is PasswordBox passwordBox)
        {
            Password = passwordBox.Password;
        }
    }

    private void CancelButton_OnClick(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    private async void ConfirmButton_OnClick(object sender, RoutedEventArgs e)
    {
        try
        {
            var isPasswordCorrect = await _apiClient.ConfirmUserPasswordAsync(Password);

            if (!isPasswordCorrect)
            {
                CustomMessageBox.Show("Niepoprawne hasło", CustomMessageBoxType.Error, "Błąd");
                return;
            }
            
            DialogResult = true;
            Close();
        }
        catch (Exception)
        {
            CustomMessageBox.Show("Wystąpił błąd podczas weryfikacji hasła, spróbuj ponownie później.",
                CustomMessageBoxType.Error, "Błąd");
        }
    }
}