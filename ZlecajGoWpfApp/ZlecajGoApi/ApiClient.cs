using System.Net;
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

    private readonly (string Name, Func<string, string> Value) _authHeader = ("Authorization", bearerToken => $"Bearer {bearerToken}");
    
    private const string IdentityEndpoint = "identity";
    private const string UsersEndpoint = "users";
    private const string OffersEndpoint = "offers";
    private const string OfferContractorEndpoint = "offerContractor";
    private const string ReviewsEndpoint = "reviews";
    private const string CategoriesEndpoint = "categories";
    private const string StatusesEndpoint = "statuses";
    private const string TypesEndpoint = "types";

    // Identity requests
    private readonly PreparedRequest _registerRequest = new($"{IdentityEndpoint}/register", Method.Post);
    private readonly PreparedRequest _loginRequest = new($"{IdentityEndpoint}/login", Method.Post);
    private readonly PreparedRequest _refreshRequest = new($"{IdentityEndpoint}/refresh", Method.Post);
    private readonly PreparedRequest _findUserNameRequest = new($"{IdentityEndpoint}/findUserName");
    private readonly PreparedRequest _isEmailExistsRequest = new($"{IdentityEndpoint}/isEmailExists");
    private readonly PreparedRequest _isPhoneNumberExistsRequest = new($"{IdentityEndpoint}/isPhoneNumberExists");
    private readonly PreparedRequest _isUserNameExistsRequest = new($"{IdentityEndpoint}/isUserNameExists");
    
    // User requests
    private readonly PreparedRequest _getUserOrUsersRequest = new(UsersEndpoint);
    private readonly PreparedRequest _getCurrentUserRequest = new($"{UsersEndpoint}/currentUser");
    private readonly PreparedRequest _updateUserRequest = new($"{UsersEndpoint}/update", Method.Patch);
    private readonly PreparedRequest _confirmUserPasswordRequest = new($"{UsersEndpoint}/confirmPassword", Method.Post);
    private readonly PreparedRequest _changeUserPasswordRequest = new($"{UsersEndpoint}/changePassword", Method.Post);
    
    // Offer requests
    private readonly PreparedRequest _getOfferOrOffersRequest = new(OffersEndpoint);
    private readonly PreparedRequest _getCurrentUserOffersRequest = new($"{OffersEndpoint}/currentUserOffers");
    private readonly PreparedRequest _createOfferRequest = new($"{OffersEndpoint}/create", Method.Post);
    private readonly PreparedRequest _updateOfferRequest = new($"{OffersEndpoint}/update", Method.Patch);
    private readonly PreparedRequest _updateOfferStatusRequest = new($"{OffersEndpoint}/updateStatus", Method.Patch);
    private readonly PreparedRequest _deleteOfferRequest = new($"{OffersEndpoint}/delete", Method.Delete);
    
    // OfferContractor requests
    private readonly PreparedRequest _getContractedOfferOrOffersRequest = new($"{OfferContractorEndpoint}");
    private readonly PreparedRequest _createContractRequest = new($"{OfferContractorEndpoint}/createContract", Method.Post);
    private readonly PreparedRequest _updateContractRequest = new($"{OfferContractorEndpoint}/updateContract", Method.Patch);
    
    // Review requests
    private readonly PreparedRequest _getReviewsRequest = new($"{ReviewsEndpoint}");
    private readonly PreparedRequest _getReceivedReviewsRequest = new($"{ReviewsEndpoint}/received");
    private readonly PreparedRequest _getWrittenReviewsRequest = new($"{ReviewsEndpoint}/written");
    private readonly PreparedRequest _getReceivedReviewsFromUserRequest = new($"{ReviewsEndpoint}/receivedFromUser");
    private readonly PreparedRequest _getWrittenReviewsForUserRequest = new($"{ReviewsEndpoint}/writtenForUser");
    private readonly PreparedRequest _createReviewRequest = new($"{ReviewsEndpoint}/create", Method.Post);
    private readonly PreparedRequest _updateReviewRequest = new($"{ReviewsEndpoint}/update", Method.Patch);
    private readonly PreparedRequest _deleteReviewRequest = new($"{ReviewsEndpoint}/delete", Method.Delete);
    
    // Other requests
    private readonly PreparedRequest _getCategoryOrCategoriesRequest = new($"{CategoriesEndpoint}");
    private readonly PreparedRequest _getStatusOrStatusesRequest = new($"{StatusesEndpoint}");
    private readonly PreparedRequest _getTypeOrTypesRequest = new($"{TypesEndpoint}");
    
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };
    
    public async Task SignUpUserAsync(SignUpDto dto)
    {
        if (await IsEmailExistsAsync(dto.Email))
        {
            throw new EmailAlreadyInUseException(dto.Email);
        }

        await ExecuteRequestAsync<object>
        (
            _registerRequest.ToRestRequest()
                .AddJsonBody(dto)
        );
        
        var logInDto = new LogInDto
        {
            Email = dto.Email,
            Password = dto.Password
        };
        
        await LogInUserAsync(logInDto);
    }
    
    public async Task<bool> LogInUserAsync(LogInDto dto)
    {
        try
        {
            dto.Email = await FindUserNameAsync(dto.Email);
        }
        catch (Exception)
        {
            throw new UnauthorizedAccessException();
        }

        var jsonDocument = await ExecuteRequestAsync<JsonDocument>
        (
            _loginRequest.ToRestRequest()
                .AddJsonBody(dto)
        );
        
        var accessToken = jsonDocument.RootElement.GetProperty("accessToken").GetString()!;
        var refreshToken = jsonDocument.RootElement.GetProperty("refreshToken").GetString()!;
        
        var user = await GetCurrentUserAsync(accessToken);
        user.AccessToken = accessToken;
        user.RefreshToken = refreshToken;
        
        SetCurrentUser(user);
        
        return user.IsProfileCompleted;
    }

    public async Task UpdateUserCredentialsAsync(UpdateUserCredentialsDto dto)
    {
        if (!string.IsNullOrWhiteSpace(dto.UserName))
        {
            if (await IsUserNameExistsAsync(dto.UserName))
            {
                throw new UsernameAlreadyInUseException(dto.UserName);
            }
        }

        if (!string.IsNullOrWhiteSpace(dto.PhoneNumber))
        {
            if (await IsPhoneNumberExistsAsync(dto.PhoneNumber))
            {
                throw new PhoneNumberAlreadyInUseException(dto.PhoneNumber);
            }
        }
        
        var currentUser = GetCurrentUser();
        var accessToken = currentUser.AccessToken;
        
        await ExecuteRequestAsync<object>
        (
            _updateUserRequest.ToRestRequest()
                .AddHeader(_authHeader.Name, _authHeader.Value(accessToken))
                .AddJsonBody(dto)
        );

        await RefreshUserAsync(currentUser);
    }
    
    public async Task<bool> CheckUserPassword(string password)
    {
        var accessToken = GetAccessToken();

        var result = await ExecuteRequestAsync<bool>
        (
            _confirmUserPasswordRequest.ToRestRequest()
                .AddHeader(_authHeader.Name, _authHeader.Value(accessToken))
                .AddJsonBody(new { Password = password })
        );
        
        return result;
    }
    
    public async Task<bool> ChangeUserPassword(ChangeUserPasswordDto dto)
    {
        var accessToken = GetAccessToken();

        var result = await ExecuteRequestAsync<bool>
        (
            _changeUserPasswordRequest.ToRestRequest()
                .AddHeader(_authHeader.Name, _authHeader.Value(accessToken))
                .AddJsonBody(dto)
        );
        
        return result;
    }
    
    public void LogOutUser() 
        => UserSession.Instance.ClearUser();

    public async Task<List<OfferDto>?> GetOffersAsync() 
        => await GetDataAsync<OfferDto>(_getOfferOrOffersRequest);

    public async Task<List<OfferDto>?> GetUserOffersAsync()
        => await GetDataAsync<OfferDto>(_getCurrentUserOffersRequest);
    
    public async Task<List<CategoryDto>> GetCategoriesAsync() 
        => (await GetDataAsync<CategoryDto>(_getCategoryOrCategoriesRequest))!;

    public async Task<List<StatusDto>> GetStatusesAsync() 
        => (await GetDataAsync<StatusDto>(_getStatusOrStatusesRequest))!;

    public async Task<List<TypeDto>> GetTypesAsync() 
        => (await GetDataAsync<TypeDto>(_getTypeOrTypesRequest))!;
    
    public async Task<List<UserDto>> GetUsersAsync() 
        => (await GetDataAsync<UserDto>(_getUserOrUsersRequest))!;

    public async Task CreateOfferAsync(OfferDto dto)
    {
        var accessToken = GetAccessToken();

        await ExecuteRequestAsync<object>
        (
            _createOfferRequest.ToRestRequest()
                .AddHeader(_authHeader.Name, _authHeader.Value(accessToken))
                .AddJsonBody(dto)
        );
        
        // TODO: Send hidden notification to users via api to refresh their offers
    }

    public async Task UpdateOfferAsync(OfferDto dto)
    {
        var accessToken = GetAccessToken();

        await ExecuteRequestAsync<object>
        (
            _updateOfferRequest.ToRestRequest()
                .AddHeader(_authHeader.Name, _authHeader.Value(accessToken))
                .AddQueryParameter("offerId", dto.Id)
                .AddJsonBody(dto)
        );
    }

    public async Task DeleteOfferAsync(OfferDto dto)
    {
        var accessToken = GetAccessToken();

        await ExecuteRequestAsync<object>
        (
            _deleteOfferRequest.ToRestRequest()
                .AddHeader(_authHeader.Name, _authHeader.Value(accessToken))
                .AddQueryParameter("offerId", dto.Id)
        );
    }
    
    private async Task RefreshUserAsync(UserDto userDto)
    {
        var jsonDocument = await ExecuteRequestAsync<JsonDocument>
        (
            _refreshRequest.ToRestRequest()
                .AddJsonBody(new { refreshToken = userDto.RefreshToken })
        );
        
        var accessToken = jsonDocument.RootElement.GetProperty("accessToken").GetString()!;
        var refreshToken = jsonDocument.RootElement.GetProperty("refreshToken").GetString()!;
        
        userDto = await GetCurrentUserAsync(accessToken);
        userDto.AccessToken = accessToken;
        userDto.RefreshToken = refreshToken;
        
        SetCurrentUser(userDto);
    }
    
    private async Task<UserDto> GetCurrentUserAsync(string accessToken)
    {
        var currentUser = await ExecuteRequestAsync<UserDto>
        (
            _getCurrentUserRequest.ToRestRequest()
                .AddHeader(_authHeader.Name, _authHeader.Value(accessToken))
        );
        
        return currentUser;
    }

    private async Task<string> FindUserNameAsync(string email)
    {
        var result = await ExecuteRequestAsync<string>
        (
            _findUserNameRequest.ToRestRequest()
                .AddQueryParameter(nameof(email), email)
        );
        
        return result;
    }
    
    private async Task<bool> IsEmailExistsAsync(string email)
    {
        var result = await ExecuteRequestAsync<bool>
        (
            _isEmailExistsRequest.ToRestRequest()
                .AddQueryParameter(nameof(email), email)
        );
        
        return result;
    }
    
    private async Task<bool> IsPhoneNumberExistsAsync(string phoneNumber)
    {
        var result = await ExecuteRequestAsync<bool>
        (
            _isPhoneNumberExistsRequest.ToRestRequest()
                .AddQueryParameter(nameof(phoneNumber), phoneNumber)
        );
        
        return result;
    }

    private async Task<bool> IsUserNameExistsAsync(string userName)
    {
        var result = await ExecuteRequestAsync<bool>
        (
            _isUserNameExistsRequest.ToRestRequest()
                .AddQueryParameter(nameof(userName), userName)
        );
        
        return result;
    }

    private async Task<List<T>?> GetDataAsync<T>(PreparedRequest request)
    {
        var accessToken = GetAccessToken();

        var data = await ExecuteRequestAsync<List<T>>
        (
            request.ToRestRequest()
                .AddHeader(_authHeader.Name, _authHeader.Value(accessToken))
        );
        
        return data;
    }
    
    private async Task<T> ExecuteRequestAsync<T>(RestRequest request)
    {        
        var response = await _client.ExecuteAsync(request);
        
        HandleResponse(response);

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

    private static void HandleResponse(RestResponse response)
    {
        if (response.StatusCode == HttpStatusCode.Unauthorized)
            throw new UnauthorizedAccessException();
        
        if (!response.IsSuccessful)
            throw new UnsuccessfulResponseException();

        if (response.Content is null)
            throw new EmptyContentException();
    }
    
    private static UserDto GetCurrentUser() => UserSession.Instance.CurrentUser;
    private static string GetAccessToken() => GetCurrentUser().AccessToken;
    private static void SetCurrentUser(UserDto userDto) => UserSession.Instance.SetUser(userDto);
}