using RestSharp;

namespace ZlecajGoApi.Helpers;

internal class PreparedRequest(string resource, Method method = Method.Get)
{
    private string Resource { get; } = resource;
    private Method Method { get; } = method;
    
    internal RestRequest ToRestRequest() => new RestRequest(Resource, Method);
}