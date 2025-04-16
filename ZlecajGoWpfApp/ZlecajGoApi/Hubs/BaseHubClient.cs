using Microsoft.AspNetCore.SignalR.Client;

namespace ZlecajGoApi.Hubs;

public interface IBaseHubClient : IAsyncDisposable
{
    Task ConnectAsync();
    Task DisconnectAsync();
}

public abstract class BaseHubClient(string hubPath) : IBaseHubClient
{
    protected readonly string HubUrl = ApiClient.BaseUrl.Replace("api/", $"hubs/{hubPath}");
    protected HubConnection? HubConnection;
    protected bool IsConnected;
    
    public async Task ConnectAsync()
    {
        if (IsConnected) return;

        var accessToken = UserSession.Instance.CurrentUser.AccessToken;

        if (string.IsNullOrEmpty(accessToken))
        {
            throw new UnauthorizedAccessException();
        }

        HubConnection = new HubConnectionBuilder()
            .WithUrl(HubUrl, options =>
            {
                options.AccessTokenProvider = () => Task.FromResult(accessToken)!;
            })
            .WithAutomaticReconnect()
            .Build();
        
        RegisterHandlers();

        await HubConnection.StartAsync();
        IsConnected = true;
    }
    
    public async Task DisconnectAsync()
    {
        if (HubConnection != null && IsConnected)
        {
            await HubConnection.StopAsync();
            IsConnected = false;
        }
    }
    
    public async ValueTask DisposeAsync()
    {
        if (HubConnection != null)
        {
            await HubConnection.DisposeAsync();
            HubConnection = null;
            IsConnected = false;
        }

        GC.SuppressFinalize(this);
    }
    
    protected abstract void RegisterHandlers();
    
    protected void ThrowIfNotConnected(Exception exception)
    {
        if (!IsConnected || HubConnection == null)
        {
            throw exception;
        }
    }
}