using MaterialDesignThemes.Wpf;

namespace ZlecajGoWpfApp.Services.Snackbar;

public class SnackbarService : ISnackbarService
{
    private static readonly TimeSpan MessageDuration = TimeSpan.FromSeconds(5);
    public SnackbarMessageQueue MessageQueue { get; } = new(MessageDuration);
    
    public void EnqueueMessage(string message)
    {
        MessageQueue.Enqueue(message, "\u2715", () => MessageQueue.Clear());
    }
}