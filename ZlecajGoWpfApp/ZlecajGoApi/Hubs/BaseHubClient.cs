using Microsoft.AspNetCore.SignalR.Client;

namespace ZlecajGoApi.Hubs;

public interface IBaseHubClient : IAsyncDisposable
{
    Task ConnectAsync();
    Task DisconnectAsync();
}

public abstract class BaseHubClient(string hubPath) : IBaseHubClient
{
    private readonly string _hubUrl = ApiClient.BaseUrl.Replace("api/", $"hubs/{hubPath}");
    private bool _isConnected;
    
    protected HubConnection? HubConnection;
    
    public async Task ConnectAsync()
    {
        if (_isConnected) return;

        var accessToken = UserSession.Instance.CurrentUser.AccessToken;

        if (string.IsNullOrEmpty(accessToken))
        {
            throw new UnauthorizedAccessException();
        }

        HubConnection = new HubConnectionBuilder()
            .WithUrl(_hubUrl, options =>
            {
                options.AccessTokenProvider = () => Task.FromResult(accessToken)!;
            })
            .WithAutomaticReconnect()
            .Build();
        
        RegisterHandlers();

        await HubConnection.StartAsync();
        _isConnected = true;
    }
    
    public async Task DisconnectAsync()
    {
        if (HubConnection != null && _isConnected)
        {
            await HubConnection.StopAsync();
            _isConnected = false;
        }
    }
    
    public async ValueTask DisposeAsync()
    {
        if (HubConnection != null)
        {
            await HubConnection.DisposeAsync();
            HubConnection = null;
            _isConnected = false;
        }

        GC.SuppressFinalize(this);
    }
    
    protected abstract void RegisterHandlers();
    
    protected void ThrowIfNotConnected(Exception exception)
    {
        if (!_isConnected || HubConnection == null)
        {
            throw exception;
        }
    }
}