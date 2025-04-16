using Microsoft.AspNetCore.SignalR.Client;
using ZlecajGoApi.Dtos;
using ZlecajGoApi.Exceptions;

namespace ZlecajGoApi.Hubs.Notification;

public class NotificationHubClient() : BaseHubClient("notification"), INotificationHubClient
{
    // Client-side method names (received from server)
    private const string ReceiveOfferContractNotificationMethod = "ReceiveOfferContractNotification";
    // Server-side method names (invoked by client)
    private const string SendOfferContractNotificationMethod = "SendOfferContractNotification";
    
    public event Func<OfferContractorDto, string, Task>? OnOfferContractNotificationReceived;
    
    protected override void RegisterHandlers()
    {
        HubConnection!.On<OfferContractorDto, string>(
            ReceiveOfferContractNotificationMethod, 
            async (offerContractorDto, message) =>
                await (OnOfferContractNotificationReceived?.Invoke(offerContractorDto, message) ?? Task.CompletedTask));
    }
    
    public async Task SendOfferContractNotificationAsync(OfferContractorDto offerContractorDto, string offerProviderId, string message)
    {
        ThrowIfNotConnected(new NotificationHubConnectionException());
        await HubConnection!.InvokeAsync(SendOfferContractNotificationMethod, offerContractorDto, offerProviderId, message);
    }
}