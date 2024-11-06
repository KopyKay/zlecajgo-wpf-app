using System.Text.Json;
using RestSharp;
using ZlecajGoApi.Dtos;
using ZlecajGoApi.Helpers;

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
    
    public async Task<bool> SignUpUserAsync(SignUpDto signUpDto)
    {
        if (await IsEmailExistsAsync(signUpDto.Email))
            throw new ArgumentException("Podany adres email jest już zarejestrowany!");
        
        const string resource = $"{IdentityEndpoint}/register";
        var request = new RestRequest(resource, Method.Post)
            .AddBody(signUpDto);
        
        var response = await _client.ExecuteAsync(request);
        RequestHelper.HandleResponse(response);
        
        var logInDto = new LogInDto { Email = signUpDto.Email, Password = signUpDto.Password };
        
        await LogInUserAsync(logInDto);
        
        return true;
    }
    
    public async Task<bool> LogInUserAsync(LogInDto logInDto)
    {
        const string resource = $"{IdentityEndpoint}/login";
        var request = new RestRequest(resource, Method.Post)
            .AddBody(logInDto);
        
        var response = await _client.ExecuteAsync(request);
        RequestHelper.HandleResponse(response);
        
        var responseContent = response.Content!;
        var jsonDocument = JsonDocument.Parse(responseContent);
        var accessToken = jsonDocument.RootElement.GetProperty("accessToken").GetString()!;
        var refreshToken = jsonDocument.RootElement.GetProperty("refreshToken").GetString()!;
        
        var user = await GetCurrentUserAsync(accessToken);
        user.AccessToken = accessToken;
        user.RefreshToken = refreshToken;
        
        if (user.IsProfileCompleted is false) return false;

        // TODO: Save user to session
        
        return true;
    }

    private async Task<UserDto> GetCurrentUserAsync(string accessToken)
    {
        const string resource = $"{UsersEndpoint}/currentUser";
        var request = new RestRequest(resource, Method.Get)
            .AddAuthorizationHeader(accessToken);
        
        var response = await _client.ExecuteAsync(request);
        RequestHelper.HandleResponse(response);
        
        var responseContent = response.Content!;
        var user = JsonSerializer.Deserialize<UserDto>(responseContent, _jsonOptions)!;
        
        return user;
    }
    
    private async Task<bool> IsEmailExistsAsync(string email)
    {
        const string resource = $"{IdentityEndpoint}/isEmailExists";
        var request = new RestRequest(resource, Method.Get)
            .AddQueryParameter(nameof(email), email);
        
        var response = await _client.ExecuteAsync(request);
        RequestHelper.HandleResponse(response);
        
        var result = bool.Parse(response.Content!);

        return result;
    }
}