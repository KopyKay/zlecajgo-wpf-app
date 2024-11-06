using System.Net;
using RestSharp;
using ZlecajGoApi.Exceptions;

namespace ZlecajGoApi.Helpers;

internal static class RequestHelper
{
    public static void HandleResponse(RestResponse response)
    {
        if (response.StatusCode == HttpStatusCode.Unauthorized)
            throw new ArgumentException("Nieautoryzowany użytkownik! Sprawdź dane logowania.");
        
        if (!response.IsSuccessful)
            throw new UnsuccessfulResponseException();
    }

    public static RestRequest AddAuthorizationHeader(this RestRequest request, string accessToken)
    {
        return request.AddHeader("Authorization", $"Bearer {accessToken}");
    }
}