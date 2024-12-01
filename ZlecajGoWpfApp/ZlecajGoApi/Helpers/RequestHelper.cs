using System.Net;
using RestSharp;
using ZlecajGoApi.Exceptions;
using UnauthorizedAccessException = ZlecajGoApi.Exceptions.UnauthorizedAccessException;

namespace ZlecajGoApi.Helpers;

internal static class RequestHelper
{
    private const string AuthorizationHeaderName = "Authorization";
    private const string BearerPrefix = "Bearer";
    
    public static void HandleResponse(RestResponse response)
    {
        if (response.StatusCode == HttpStatusCode.Unauthorized)
            throw new UnauthorizedAccessException();
        
        if (!response.IsSuccessful)
            throw new UnsuccessfulResponseException();

        if (response.Content is null)
            throw new EmptyContentException();
    }

    public static RestRequest AddAuthorizationHeader(this RestRequest request, string bearerToken)
    {
        return request.AddHeader(AuthorizationHeaderName, $"{BearerPrefix} {bearerToken}");
    }
}