using System.Text.Json;
using RestSharp;
using ZlecajGoApi.Dtos;

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
    
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

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

    public async Task<UserDto?> LogInUserAsync(LogInDto logInDto)
    {
        const string logInResource = $"{IdentityEndpoint}/login";
        var logInRequest = new RestRequest(logInResource, Method.Post);
        logInRequest.AddJsonBody(logInDto);
        
        var logInResponse = await _client.ExecuteAsync(logInRequest);
        if (!logInResponse.IsSuccessful) return null;
        
        var responseContent = logInResponse.Content;
        var jsonDocument = JsonDocument.Parse(responseContent!);
        var accessToken = jsonDocument.RootElement.GetProperty("accessToken").GetString()!;
        var refreshToken = jsonDocument.RootElement.GetProperty("refreshToken").GetString()!;
        var id = await GetCurrentUserIdAsync(accessToken);

        var user = new UserDto
        {
            Id = id,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            Email = logInDto.Email,
            Password = logInDto.Password
        };

        await GetCurrentUser(user);

        return user;
    }

    private async Task<string> GetCurrentUserIdAsync(string accessToken)
    {
        const string currentUserIdResource = $"{UsersEndpoint}/currentUserId";
        var currentUserIdRequest = new RestRequest(currentUserIdResource, Method.Get);
        currentUserIdRequest.AddHeader("Authorization", $"Bearer {accessToken}");

        var currentUserIdResponse = await _client.ExecuteAsync(currentUserIdRequest);

        var responseContent = currentUserIdResponse.Content!;
        var jsonDocument = JsonDocument.Parse(responseContent);
        var userId = jsonDocument.RootElement.GetString()!;

        return userId;
    }

    private async Task GetCurrentUser(UserDto userDto)
    {
        var currentUserResource = $"{UsersEndpoint}/{userDto.Id}";
        var currentUserRequest = new RestRequest(currentUserResource, Method.Get);
        currentUserRequest.AddHeader("Authorization", $"Bearer {userDto.AccessToken}");
        
        var currentUserResponse = await _client.ExecuteAsync(currentUserRequest);
        
        var responseContent = currentUserResponse.Content!;
        var currentUser = JsonSerializer.Deserialize<UserDto>(responseContent, _jsonOptions)!;
        
        foreach (var property in typeof(UserDto).GetProperties())
        {
            if (property.GetValue(userDto) is null)
                property.SetValue(userDto, property.GetValue(currentUser));
        }
    }
}