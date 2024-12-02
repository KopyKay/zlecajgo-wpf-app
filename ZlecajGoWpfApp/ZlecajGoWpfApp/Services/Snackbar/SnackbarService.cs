using MaterialDesignThemes.Wpf;

namespace ZlecajGoWpfApp.Services.Snackbar;

public class SnackbarService : ISnackbarService
{
    private static readonly TimeSpan MessageDuration = TimeSpan.FromSeconds(5);
    public SnackbarMessageQueue MessageQueue { get; } = new(MessageDuration);
    
    public void EnqueueMessage(string message)
    {
        var closeIcon = new PackIcon { Kind = PackIconKind.CloseCircle};
        MessageQueue.Enqueue(message, closeIcon, () => { });
    }
}