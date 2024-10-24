using System.Text.Json.Nodes;
using RestSharp;

namespace ZlecajGoApi;

public class ApiClient : IApiClient
{
    private const string BaseUrl = "https://localhost:7130/api/";
    private readonly RestClient _client = new(BaseUrl);
    
    private const string IdentityEndpoint = "identity";
    private const string UsersEndpoint = "users";
    private const string OffersEndpoint = "offers";
    private const string OfferContractorEndpoint = "offerContractor";
    private const string ReviewsEndpoint = "reviews";
    private const string CategoriesEndpoint = "categories";
    private const string StatusesEndpoint = "statuses";
    private const string TypesEndpoint = "types";

    public Task<RestResponse> SignUpUserAsync(string email, string password)
    {
        // const string resource = $"{IdentityEndpoint}/register";
        // var request = new RestRequest(resource, Method.Post);
        // request.AddJsonBody(new { Email = email, Password = password });
        //
        // var response = await _client.ExecuteAsync(request);
        //
        // if (!response.IsSuccessful)
        // {
        //     Console.WriteLine("Request failed");
        //     Console.WriteLine("Error: " + response.Content);
        //     return response;
        // }
        //
        // var logInResponse = await LogInUserAsync(email, password);
        //
        // return logInResponse;
        throw new NotImplementedException();
    }

    public Task<string> LogInUserAsync(string email, string password)
    {
        // return whole user object with token
        throw new NotImplementedException();
    }

    public Task<string> GetCurrentUserIdAsync(string token)
    {
        // remove this method later and merge it with LogInUser method
        //request.AddHeader("Authorization", $"Bearer {token}");
        throw new NotImplementedException();
    }
}