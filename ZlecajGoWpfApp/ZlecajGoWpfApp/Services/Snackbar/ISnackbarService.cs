using MaterialDesignThemes.Wpf;

namespace ZlecajGoWpfApp.Services.Snackbar;

public interface ISnackbarService
{
    SnackbarMessageQueue MessageQueue { get; }
    void EnqueueMessage(string message);
}