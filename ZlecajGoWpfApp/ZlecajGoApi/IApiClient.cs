using ZlecajGoApi.Dtos;

namespace ZlecajGoApi;

public interface IApiClient
{
    Task SignUpUserAsync(SignUpDto dto);
    Task<bool> LogInUserAsync(LogInDto dto);
    Task UpdateUserCredentialsAsync(UpdateUserCredentialsDto dto);
}