using RestSharp;

namespace ZlecajGoApi;

public interface IApiClient
{
    Task<RestResponse> SignUpUserAsync(string email, string password);
    Task<string> LogInUserAsync(string email, string password);
    Task<string> GetCurrentUserIdAsync(string token);
}