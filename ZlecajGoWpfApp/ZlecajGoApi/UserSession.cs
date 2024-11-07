using ZlecajGoApi.Dtos;

namespace ZlecajGoApi;

internal class UserSession
{
    private static UserSession? _instance;
    private static readonly object InstanceLock = new();

    public UserDto? CurrentUser { get; private set; }

    private UserSession() { }
    
    public static UserSession Instance
    {
        get
        {
            lock (InstanceLock)
            {
                return _instance ??= new UserSession();
            }
        }
    }
    
    public void SetUser(UserDto user)
    {
        CurrentUser = user;
    }

    public void ClearUser()
    {
        CurrentUser = null;
    }
}