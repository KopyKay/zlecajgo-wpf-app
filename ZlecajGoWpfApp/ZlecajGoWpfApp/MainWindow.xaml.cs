using System.Windows;
using ZlecajGoWpfApp.Services;
using ZlecajGoWpfApp.Views;

namespace ZlecajGoWpfApp;

public partial class MainWindow : Window
{
    public MainWindow(SnackbarService snackbarService)
    {
        InitializeComponent();
        MainSnackbar.MessageQueue = snackbarService.MessageQueue;
    }
}