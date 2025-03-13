using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using MaterialDesignThemes.Wpf;
using ZlecajGoWpfApp.Enums;

namespace ZlecajGoWpfApp.CustomControls;

public partial class CustomMessageBox : BaseDialog
{
    public CustomMessageBox()
    {
        InitializeComponent();
    }
    
    public static void Show(string message, CustomMessageBoxType type, string? title = null)
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
                    CustomMessageBoxType.Information => PackIconKind.InformationOutline,
                    CustomMessageBoxType.Confirmation => PackIconKind.CheckCircleOutline,
                    CustomMessageBoxType.Warning => PackIconKind.AlertOutline,
                    CustomMessageBoxType.Error => PackIconKind.CloseCircleOutline,
                    _ => PackIconKind.AlertBoxOutline
                },
                Foreground = type switch
                {
                    CustomMessageBoxType.Information => Brushes.Blue,
                    CustomMessageBoxType.Confirmation => Brushes.Green,
                    CustomMessageBoxType.Warning => Brushes.Orange,
                    CustomMessageBoxType.Error => Brushes.Red,
                    _ => Brushes.BlueViolet
                }
            }
        };

        cmb.ShowDialog();
    }

    private void Button_OnClick(object sender, RoutedEventArgs e) => Close();
}