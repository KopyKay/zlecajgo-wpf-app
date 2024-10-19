using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using ZlecajGoWpfApp.Services;
using ZlecajGoWpfApp.View;

namespace ZlecajGoWpfApp.ViewModel;

public partial class SignUpViewModel : BaseViewModel
{
    private readonly NavigationService _navigationService;
    
    public SignUpViewModel(NavigationService navigationService)
    {
        _navigationService = navigationService;
        Title = "Rejestracja";
    }
    
    [RelayCommand]
    private void GoToLogIn()
    {
        var logInPage = App.AppHost.Services.GetRequiredService<LogInPage>();
        _navigationService.NavigateTo(logInPage);
    }

    [RelayCommand]
    private void GoToSetUpUserCredentials()
    {
        var setUpUserCredentialsPage = App.AppHost.Services.GetRequiredService<SetUpUserCredentialsPage>();
        _navigationService.NavigateTo(setUpUserCredentialsPage);
    }
}