using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using ZlecajGoApi;
using ZlecajGoWpfApp.Services;
using ZlecajGoWpfApp.Views;

namespace ZlecajGoWpfApp.ViewModels;

public partial class SignUpViewModel : BaseViewModel
{
    public SignUpViewModel(NavigationService navigationService, SnackbarService snackbarService, IApiClient apiClient) 
        : base(navigationService, snackbarService, apiClient)
    {
        Title = "Rejestracja";
    }
    
    [RelayCommand]
    private void GoToLogIn() => NavigationService.NavigateTo<LogInPage>();

    [RelayCommand]
    private void GoToSetUpUserCredentials() => NavigationService.NavigateTo<SetUpUserCredentialsPage>();
}