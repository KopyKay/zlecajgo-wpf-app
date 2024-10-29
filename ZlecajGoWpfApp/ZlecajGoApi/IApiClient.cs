using RestSharp;
using ZlecajGoApi.Dtos;

namespace ZlecajGoApi;

public interface IApiClient
{
    Task<RestResponse> SignUpUserAsync(string email, string password);
    Task<UserDto?> LogInUserAsync(LogInDto logInDto);
}