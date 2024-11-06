using ZlecajGoApi.Dtos;

namespace ZlecajGoApi;

public interface IApiClient
{
    Task<bool> SignUpUserAsync(SignUpDto signUpDto);
    Task<bool> LogInUserAsync(LogInDto logInDto);
}