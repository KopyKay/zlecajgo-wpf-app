using Microsoft.AspNetCore.SignalR.Client;
using ZlecajGoApi.Dtos;
using ZlecajGoApi.Exceptions;

namespace ZlecajGoApi.Hubs.Chat;

public class ChatHubClient() : BaseHubClient("chat"), IChatHubClient
{
    // Client-side method names (received from server)
    private const string ReceiveChatMethod = "ReceiveChat";
    private const string ReceiveMessageMethod = "ReceiveMessage";
    private const string ReceiveReadMessagesMethod = "ReceiveReadMessages";
    
    // Server-side method names (invoked by client)
    private const string CreateChatMethod = "CreateChat";
    private const string SendMessageMethod = "SendMessage";
    private const string ReadMessagesMethod = "ReadMessages";

    public event Func<ChatDto, Task>? OnChatReceived;
    public event Func<MessageDto, Task>? OnMessageReceived;
    public event Func<List<Guid>, Task>? OnMessagesReadReceived;
    
    protected override void RegisterHandlers()
    {
        HubConnection!.On<ChatDto>(ReceiveChatMethod, async chat => 
            await (OnChatReceived?.Invoke(chat) ?? Task.CompletedTask));

        HubConnection!.On<MessageDto>(ReceiveMessageMethod, async message => 
            await (OnMessageReceived?.Invoke(message) ?? Task.CompletedTask));

        HubConnection!.On<List<Guid>>(ReceiveReadMessagesMethod, async messageIds => 
            await (OnMessagesReadReceived?.Invoke(messageIds) ?? Task.CompletedTask));
    }

    public async Task CreateChatAsync(string recipientId, string initialMessage = "")
    {
        ThrowIfNotConnected(new ChatHubConnectionException());
        await HubConnection!.InvokeAsync(CreateChatMethod, recipientId, initialMessage);
    }

    public async Task SendMessageAsync(Guid chatId, string content)
    {
        ThrowIfNotConnected(new ChatHubConnectionException());
        await HubConnection!.InvokeAsync(SendMessageMethod, chatId, content);
    }

    public async Task ReadMessagesAsync(Guid chatId)
    {
        ThrowIfNotConnected(new ChatHubConnectionException());
        await HubConnection!.InvokeAsync(ReadMessagesMethod, chatId);
    }
}