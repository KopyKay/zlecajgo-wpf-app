using System.Windows;
using ZlecajGoWpfApp.Services.Snackbar;

namespace ZlecajGoWpfApp;

public partial class MainWindow : Window
{
    public MainWindow(ISnackbarService snackbarService)
    {
        InitializeComponent();
        MainSnackbar.MessageQueue = snackbarService.MessageQueue;
    }
}