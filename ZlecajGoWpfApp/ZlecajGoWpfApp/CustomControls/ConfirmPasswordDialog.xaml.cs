using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using ZlecajGoApi;
using ZlecajGoWpfApp.Enums;

namespace ZlecajGoWpfApp.CustomControls;

public partial class ConfirmPasswordDialog : Window
{
    public ConfirmPasswordDialog(IApiClient apiClient)
    {
        InitializeComponent();
        
        _apiClient = apiClient;
    }
    
    private readonly IApiClient _apiClient;
    
    private string Password { get; set; } = string.Empty;
    public string EnteredPassword => Password;

    #region Hide window default buttons
    [DllImport("user32.dll")]
    private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

    private const int GWL_STYLE = -16;
    private const int WS_SYSMENU = 0x80000;

    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);
        IntPtr hwnd = new WindowInteropHelper(this).Handle;
        int currentStyle = GetWindowLong(hwnd, GWL_STYLE);
        SetWindowLong(hwnd, GWL_STYLE, currentStyle & ~WS_SYSMENU);
    }
    #endregion

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
        var isPasswordCorrect = await _apiClient.CheckUserPassword(Password);

        if (!isPasswordCorrect)
        {
            CustomMessageBox.Show("Niepoprawne hasło", CustomMessageBoxType.Error, "Błąd");
            return;
        }
        
        DialogResult = true;
        Close();
    }
}