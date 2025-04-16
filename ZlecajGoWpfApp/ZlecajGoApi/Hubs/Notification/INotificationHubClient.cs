using ZlecajGoApi.Dtos;

namespace ZlecajGoApi.Hubs.Notification;

public interface INotificationHubClient : IBaseHubClient
{
    event Func<OfferContractorDto, string, Task> OnOfferContractNotificationReceived;
    Task SendOfferContractNotificationAsync(OfferContractorDto offerContractorDto, string offerProviderId, string message);
}