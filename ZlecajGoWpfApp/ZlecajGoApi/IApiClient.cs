using RestSharp;
using ZlecajGoApi.Models;

namespace ZlecajGoApi;

public interface IApiClient
{
    Task<RestResponse> SignUpUserAsync(string email, string password);
    Task<User?> LogInUserAsync(string email, string password);
}