using ZlecajGoApi.Dtos;

namespace ZlecajGoApi;

internal class UserSession
{
    private static UserSession? _instance;
    private static readonly object InstanceLock = new();

    private UserDto? _currentUser;

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
    
    public UserDto CurrentUser
    {
        get
        {
            if (_currentUser is null)
                throw new InvalidOperationException("Użytkownik nie jest zalogowany!");
            return _currentUser;
        }
        private set => _currentUser = value;
    }
    
    public void SetUser(UserDto user)
    {
        CurrentUser = user;
    }

    public void ClearUser()
    {
        _currentUser = null;
    }
}