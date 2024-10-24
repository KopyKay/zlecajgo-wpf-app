using ZlecajGoApi;
using ZlecajGoWpfApp.Services;

namespace ZlecajGoWpfApp.ViewModel;

public class SetUpUserCredentialsViewModel : BaseViewModel
{
    public SetUpUserCredentialsViewModel(NavigationService navigationService, SnackbarService snackbarService, IApiClient apiClient) 
        : base(navigationService, snackbarService, apiClient)
    {
        Title = "Uzupełnienie danych użytkownika";
    }
}