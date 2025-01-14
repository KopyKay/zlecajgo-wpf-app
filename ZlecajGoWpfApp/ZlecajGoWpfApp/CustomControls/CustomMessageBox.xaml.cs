using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using MaterialDesignThemes.Wpf;
using ZlecajGoWpfApp.Enums;

namespace ZlecajGoWpfApp.CustomControls;

public partial class CustomMessageBox : Window
{
    public CustomMessageBox()
    {
        InitializeComponent();
    }

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
    
    public static void Show(string message, CustomMessageBoxTypes type, string? title = null)
    {
        title ??= "Powiadomienie";
        
        var cmb = new CustomMessageBox
        {
            Owner = Application.Current.MainWindow,
            Title = title,
            Message =
            {
                Text = message
            },
            Icon =
            {
                Kind = type switch
                {
                    CustomMessageBoxTypes.Information => PackIconKind.InformationOutline,
                    CustomMessageBoxTypes.Confirmation => PackIconKind.CheckCircleOutline,
                    CustomMessageBoxTypes.Warning => PackIconKind.AlertOutline,
                    CustomMessageBoxTypes.Error => PackIconKind.CloseCircleOutline,
                    _ => PackIconKind.AlertBoxOutline
                },
                Foreground = type switch
                {
                    CustomMessageBoxTypes.Information => Brushes.Blue,
                    CustomMessageBoxTypes.Confirmation => Brushes.Green,
                    CustomMessageBoxTypes.Warning => Brushes.Orange,
                    CustomMessageBoxTypes.Error => Brushes.Red,
                    _ => Brushes.BlueViolet
                }
            }
        };

        cmb.ShowDialog();
    }

    private void Button_OnClick(object sender, RoutedEventArgs e) => Close();
}