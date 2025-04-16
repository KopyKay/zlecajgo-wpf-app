using Microsoft.AspNetCore.SignalR.Client;
using ZlecajGoApi.Dtos;
using ZlecajGoApi.Exceptions;
using UnauthorizedAccessException = System.UnauthorizedAccessException;

namespace ZlecajGoApi.Hubs.Chat;

public class ChatHubClient : IChatHubClient, IAsyncDisposable
{
    private readonly string _hubUrl = ApiClient.BaseUrl.Replace("api/", "hubs/chat");
    private HubConnection? _hubConnection;
    private bool _isConnected;

    public event Func<ChatDto, Task>? OnChatReceived;
    public event Func<MessageDto, Task>? OnMessageReceived;
    public event Func<List<Guid>, Task>? OnMessagesReadReceived;
    
    // Client-side method names (received from server)
    private const string ReceiveChatMethod = "ReceiveChat";
    private const string ReceiveMessageMethod = "ReceiveMessage";
    private const string ReceiveReadMessagesMethod = "ReceiveReadMessages";
    
    // Server-side method names (invoked by client)
    private const string CreateChatMethod = "CreateChat";
    private const string SendMessageMethod = "SendMessage";
    private const string ReadMessagesMethod = "ReadMessages";
    
    public async Task ConnectAsync()
    {
        if (_isConnected) return;

        var accessToken = UserSession.Instance.CurrentUser.AccessToken;

        if (string.IsNullOrEmpty(accessToken))
        {
            throw new UnauthorizedAccessException();
        }
        
        _hubConnection = new HubConnectionBuilder()
            .WithUrl(_hubUrl, options =>
            {
                options.AccessTokenProvider = () => Task.FromResult(accessToken)!;
            })
            .WithAutomaticReconnect()
            .Build();

        _hubConnection.On<ChatDto>(ReceiveChatMethod, async chat => 
            await (OnChatReceived?.Invoke(chat) ?? Task.CompletedTask));

        _hubConnection.On<MessageDto>(ReceiveMessageMethod, async message => 
            await (OnMessageReceived?.Invoke(message) ?? Task.CompletedTask));

        _hubConnection.On<List<Guid>>(ReceiveReadMessagesMethod, async messageIds => 
            await (OnMessagesReadReceived?.Invoke(messageIds) ?? Task.CompletedTask));
        
        await _hubConnection.StartAsync();
        _isConnected = true;
    }

    public async Task DisconnectAsync()
    {
        if (_hubConnection != null && _isConnected)
        {
            await _hubConnection.StopAsync();
            _isConnected = false;
        }
    }

    public async Task CreateChatAsync(string recipientId, string initialMessage = "")
    {
        ThrowIfNotConnected();
        await _hubConnection!.InvokeAsync(CreateChatMethod, recipientId, initialMessage);
    }

    public async Task SendMessageAsync(Guid chatId, string content)
    {
        ThrowIfNotConnected();
        await _hubConnection!.InvokeAsync(SendMessageMethod, chatId, content);
    }

    public async Task ReadMessagesAsync(Guid chatId)
    {
        ThrowIfNotConnected();
        await _hubConnection!.InvokeAsync(ReadMessagesMethod, chatId);
    }
    
    public async ValueTask DisposeAsync()
    {
        if (_hubConnection != null)
        {
            await _hubConnection.DisposeAsync();
            _hubConnection = null;
            _isConnected = false;
        }
        
        GC.SuppressFinalize(this);
    }
    
    private void ThrowIfNotConnected()
    {
        if (!_isConnected || _hubConnection == null)
        {
            throw new ChatHubConnectionException();
        }
    }
}