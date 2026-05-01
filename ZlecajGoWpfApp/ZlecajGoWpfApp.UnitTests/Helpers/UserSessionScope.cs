using ZlecajGoApi;
using ZlecajGoApi.Dtos;

namespace ZlecajGoWpfApp.UnitTests.Helpers;

public sealed class UserSessionScope : IDisposable
{
    public UserSessionScope(UserDto user)
    {
        UserSession.Instance.SetUser(user);
    }

    public void Dispose()
    {
        UserSession.Instance.ClearUser();
    }
}