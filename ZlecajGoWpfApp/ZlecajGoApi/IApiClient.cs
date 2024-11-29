using ZlecajGoApi.Dtos;

namespace ZlecajGoApi;

public interface IApiClient
{
    Task SignUpUserAsync(SignUpDto dto);
    Task<bool> LogInUserAsync(LogInDto dto);
    Task UpdateUserCredentialsAsync(UpdateUserCredentialsDto dto);
    void LogOutUser();
    Task<List<OfferDto>> GetOffersAsync();
    Task<List<CategoryDto>> GetCategoriesAsync();
    Task<List<StatusDto>> GetStatusesAsync();
    Task<List<TypeDto>> GetTypesAsync();
}