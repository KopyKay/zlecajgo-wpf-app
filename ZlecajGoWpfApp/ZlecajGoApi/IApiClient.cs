using ZlecajGoApi.Dtos;

namespace ZlecajGoApi;

public interface IApiClient
{
    Task RegisterAsync(SignUpDto dto);
    Task<bool> LoginAsync(LogInDto dto);
    Task UpdateUserAsync(UpdateUserCredentialsDto dto);
    void LogOutUser();
    Task<bool> ConfirmUserPasswordAsync(string password);
    Task<bool> ChangeUserPasswordAsync(ChangeUserPasswordDto dto);
    Task<List<OfferDto>?> GetOffersAsync();
    Task<OfferDto?> GetOfferAsync(Guid id);
    Task<List<OfferDto>?> GetCurrentUserOffersAsync();
    Task<List<ChatDto>?> GetChatsAsync();
    Task<ChatDto?> GetChatAsync(Guid chatId);
    Task<List<CategoryDto>> GetCategoriesAsync();
    Task<List<StatusDto>> GetStatusesAsync();
    Task<List<TypeDto>> GetTypesAsync();
    Task<List<UserDto>> GetUsersAsync();
    Task<UserDto?> GetUserAsync(string id);
    Task CreateOfferAsync(OfferDto dto);
    Task UpdateOfferAsync(OfferDto dto);
    Task DeleteOfferAsync(OfferDto dto);
}