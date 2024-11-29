using System.Net;
using RestSharp;
using ZlecajGoApi.Exceptions;
using UnauthorizedAccessException = ZlecajGoApi.Exceptions.UnauthorizedAccessException;

namespace ZlecajGoApi.Helpers;

internal static class RequestHelper
{
    public static void HandleResponse(RestResponse response)
    {
        if (response.StatusCode == HttpStatusCode.Unauthorized)
            throw new UnauthorizedAccessException();
        
        if (!response.IsSuccessful)
            throw new UnsuccessfulResponseException();

        if (response.Content is null)
            throw new EmptyContentException();
    }

    public static RestRequest AddAuthorizationHeader(this RestRequest request, string accessToken)
    {
        return request.AddHeader("Authorization", $"Bearer {accessToken}");
    }
}