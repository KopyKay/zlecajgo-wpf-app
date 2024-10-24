using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using ZlecajGoApi;
using ZlecajGoWpfApp.Services;
using ZlecajGoWpfApp.View;

namespace ZlecajGoWpfApp.ViewModel;

public partial class SignUpViewModel : BaseViewModel
{
    public SignUpViewModel(NavigationService navigationService, SnackbarService snackbarService, IApiClient apiClient) 
        : base(navigationService, snackbarService, apiClient)
    {
        Title = "Rejestracja";
    }
    
    [RelayCommand]
    private void GoToLogIn()
    {
        var logInPage = App.AppHost.Services.GetRequiredService<LogInPage>();
        NavigationService.NavigateTo(logInPage);
    }

    [RelayCommand]
    private void GoToSetUpUserCredentials()
    {
        var setUpUserCredentialsPage = App.AppHost.Services.GetRequiredService<SetUpUserCredentialsPage>();
        NavigationService.NavigateTo(setUpUserCredentialsPage);
    }
}