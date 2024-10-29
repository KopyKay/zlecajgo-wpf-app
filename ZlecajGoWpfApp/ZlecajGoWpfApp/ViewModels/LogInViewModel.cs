using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using ZlecajGoApi;
using ZlecajGoWpfApp.Services;
using ZlecajGoWpfApp.Views;

namespace ZlecajGoWpfApp.ViewModels;

public partial class LogInViewModel : BaseViewModel
{
    public LogInViewModel(NavigationService navigationService, SnackbarService snackbarService, IApiClient apiClient) 
        : base(navigationService, snackbarService, apiClient)
    {
        Title = "Logowanie";
    }
    
    [ObservableProperty]
    private string _email = string.Empty;
    
    [ObservableProperty]
    private string _password = string.Empty;
    
    [RelayCommand]
    private async Task LogIn()
    {
        try
        {
            IsBusy = true;
            
            var user = await ApiClient.LogInUserAsync(Email, Password);
            
            if (user == null)
            {
                SnackbarService.EnqueueMessage("Niepoprawne dane logowania");
                return;
            }
            
            SnackbarService.EnqueueMessage("Zalogowano pomyślnie");
        }
        finally
        {
            IsBusy = false;
        }
    }
    
    [RelayCommand]
    private void GoToSignUp()
    {
        var singUpPage = App.AppHost.Services.GetRequiredService<SignUpPage>();
        NavigationService.NavigateTo(singUpPage);
    }
}