using ZlecajGoWpfApp.Services;

namespace ZlecajGoWpfApp.ViewModel;

public class SetUpUserCredentialsViewModel : BaseViewModel
{
    private readonly NavigationService _navigationService;
    
    public SetUpUserCredentialsViewModel(NavigationService navigationService)
    {
        _navigationService = navigationService;
        Title = "Uzupełnienie danych użytkownika";
    }
}