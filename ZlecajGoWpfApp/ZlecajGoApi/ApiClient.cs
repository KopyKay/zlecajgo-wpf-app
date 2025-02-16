using System.Text.Json;
using RestSharp;
using ZlecajGoApi.Dtos;
using ZlecajGoApi.Exceptions;
using ZlecajGoApi.Helpers;
using UnauthorizedAccessException = ZlecajGoApi.Exceptions.UnauthorizedAccessException;

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
    
    public async Task SignUpUserAsync(SignUpDto dto)
    {
        if (await IsEmailExistsAsync(dto.Email))
            throw new ArgumentException("Podany adres email jest już zarejestrowany!");
        
        const string resource = $"{IdentityEndpoint}/register";
        var request = new RestRequest(resource, Method.Post)
            .AddJsonBody(dto);
        
        await ExecuteRequestAsync<object>(request);
        
        var logInDto = new LogInDto { Email = dto.Email, Password = dto.Password };
        
        await LogInUserAsync(logInDto);
    }
    
    public async Task<bool> LogInUserAsync(LogInDto dto)
    {
        const string findUserNameEndpoint = $"{IdentityEndpoint}/findUserName";
        var findUserNameRequest = new RestRequest(findUserNameEndpoint, Method.Get)
            .AddQueryParameter(nameof(dto.Email), dto.Email);

        try { dto.Email = await ExecuteRequestAsync<string>(findUserNameRequest); }
        catch (UnsuccessfulResponseException) { throw new UnauthorizedAccessException(); }
        
        const string resource = $"{IdentityEndpoint}/login";
        var request = new RestRequest(resource, Method.Post)
            .AddJsonBody(dto);
        
        var jsonDocument = await ExecuteRequestAsync<JsonDocument>(request);
        var accessToken = jsonDocument.RootElement.GetProperty("accessToken").GetString()!;
        var refreshToken = jsonDocument.RootElement.GetProperty("refreshToken").GetString()!;
        
        var user = await GetCurrentUserAsync(accessToken);
        user.AccessToken = accessToken;
        user.RefreshToken = refreshToken;
        
        UserSession.Instance.SetUser(user);
        
        return user.IsProfileCompleted;
    }

    public async Task UpdateUserCredentialsAsync(UpdateUserCredentialsDto dto)
    {
        if (await IsUserNameExistsAsync(dto.UserName!))
            throw new ArgumentException("Podana nazwa użytkownika jest już zajęta!");
        
        if (await IsPhoneNumberExistsAsync(dto.PhoneNumber!))
            throw new ArgumentException("Podany numer telefonu jest już zarejestrowany!");
        
        var currentUser = UserSession.Instance.CurrentUser;
        
        const string resource = $"{UsersEndpoint}/update";
        var request = new RestRequest(resource, Method.Patch)
            .AddAuthorizationHeader(currentUser.AccessToken)
            .AddJsonBody(dto);
        
        await ExecuteRequestAsync<object>(request);

        await RefreshUserAsync(currentUser);
    }
    
    public void LogOutUser() 
        => UserSession.Instance.ClearUser();

    public async Task<List<OfferDto>?> GetOffersAsync() 
        => await GetDataAsync<OfferDto>(OffersEndpoint);

    public async Task<List<CategoryDto>> GetCategoriesAsync() 
        => (await GetDataAsync<CategoryDto>(CategoriesEndpoint))!;

    public async Task<List<StatusDto>> GetStatusesAsync() 
        => (await GetDataAsync<StatusDto>(StatusesEndpoint))!;

    public async Task<List<TypeDto>> GetTypesAsync() 
        => (await GetDataAsync<TypeDto>(TypesEndpoint))!;
    
    public async Task<List<UserDto>> GetUsersAsync() 
        => (await GetDataAsync<UserDto>(UsersEndpoint))!;

    public async Task CreateOfferAsync(OfferDto dto)
    {
        var currentUser = UserSession.Instance.CurrentUser;
        
        const string resource = $"{OffersEndpoint}/create";
        var request = new RestRequest(resource, Method.Post)
            .AddAuthorizationHeader(currentUser.AccessToken)
            .AddJsonBody(dto);
        
        await ExecuteRequestAsync<object>(request);
        
        // TODO: Send hidden notification to users via api to refresh their offers
    }
    
    private async Task RefreshUserAsync(UserDto userDto)
    {
        const string resource = $"{IdentityEndpoint}/refresh";
        var request = new RestRequest(resource, Method.Post)
            .AddJsonBody(new { refreshToken = userDto.RefreshToken });
        
        var jsonDocument = await ExecuteRequestAsync<JsonDocument>(request);
        var accessToken = jsonDocument.RootElement.GetProperty("accessToken").GetString()!;
        var refreshToken = jsonDocument.RootElement.GetProperty("refreshToken").GetString()!;
        
        userDto.AccessToken = accessToken;
        userDto.RefreshToken = refreshToken;
    }
    
    private async Task<UserDto> GetCurrentUserAsync(string accessToken)
    {
        const string resource = $"{UsersEndpoint}/currentUser";
        var request = new RestRequest(resource)
            .AddAuthorizationHeader(accessToken);
        
        var user = await ExecuteRequestAsync<UserDto>(request);
        return user;
    }
    
    private async Task<bool> IsEmailExistsAsync(string email)
    {
        const string resource = $"{IdentityEndpoint}/isEmailExists";
        var request = new RestRequest(resource)
            .AddQueryParameter(nameof(email), email);
        
        var result = await ExecuteRequestAsync<bool>(request);
        return result;
    }
    
    private async Task<bool> IsPhoneNumberExistsAsync(string phoneNumber)
    {
        const string resource = $"{IdentityEndpoint}/isPhoneNumberExists";
        var request = new RestRequest(resource)
            .AddQueryParameter(nameof(phoneNumber), phoneNumber);
        
        var result = await ExecuteRequestAsync<bool>(request);
        return result;
    }

    private async Task<bool> IsUserNameExistsAsync(string userName)
    {
        const string resource = $"{IdentityEndpoint}/isUserNameExists";
        var request = new RestRequest(resource)
            .AddQueryParameter(nameof(userName), userName);
        
        var result = await ExecuteRequestAsync<bool>(request);
        return result;
    }

    private async Task<List<T>?> GetDataAsync<T>(string endpoint)
    {
        var currentUser = UserSession.Instance.CurrentUser;
        
        var request = new RestRequest(endpoint)
            .AddAuthorizationHeader(currentUser.AccessToken);
        
        var data = await ExecuteRequestAsync<List<T>>(request);
        return data;
    }
    
    private async Task<T> ExecuteRequestAsync<T>(RestRequest request)
    {        
        var response = await _client.ExecuteAsync(request);
        RequestHelper.HandleResponse(response);

        if (string.IsNullOrWhiteSpace(response.Content))
        {
            if (typeof(T) == typeof(object) || typeof(T) == typeof(void))
                return default!;

            throw new EmptyContentException();
        }

        var responseContent = response.Content!;
        var data = JsonSerializer.Deserialize<T>(responseContent, _jsonOptions)!;

        return data;
    }
}