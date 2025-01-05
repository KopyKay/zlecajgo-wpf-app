using System.Windows;
using System.Windows.Media.Effects;
using ZlecajGoWpfApp.Services.Snackbar;

namespace ZlecajGoWpfApp;

public partial class MainWindow : Window
{
    public MainWindow(ISnackbarService snackbarService)
    {
        InitializeComponent();
        MainSnackbar.MessageQueue = snackbarService.MessageQueue;
    }

    private void MainWindow_OnDeactivated(object? sender, EventArgs e)
    {
        var blurEffect = new BlurEffect();
        BlurWindow.Effect = blurEffect;
    }

    private void MainWindow_OnActivated(object? sender, EventArgs e)
    {
        BlurWindow.Effect = null;
    }
}