using ZlecajGoApi.Dtos;

namespace ZlecajGoApi.Hubs.Chat;

public interface IChatHubClient
{
    event Func<ChatDto, Task> OnChatReceived;
    event Func<MessageDto, Task> OnMessageReceived;
    event Func<List<Guid>, Task> OnMessagesReadReceived;
    
    Task ConnectAsync();
    Task DisconnectAsync();
    Task CreateChatAsync(string recipientId, string initialMessage = "");
    Task SendMessageAsync(Guid chatId, string content);
    Task ReadMessagesAsync(Guid chatId);
}