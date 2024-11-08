using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ZlecajGoApi;
using ZlecajGoApi.Dtos;
using ZlecajGoApi.Exceptions;
using ZlecajGoWpfApp.Helpers;
using ZlecajGoWpfApp.Services;
using ZlecajGoWpfApp.Views;
using UnauthorizedAccessException = ZlecajGoApi.Exceptions.UnauthorizedAccessException;

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
            await TryLogInAsync();
        }
        catch (ArgumentException e)
        {
            SnackbarService.EnqueueMessage(e.Message);
        }
        catch (UnauthorizedAccessException e)
        {
            SnackbarService.EnqueueMessage(e.Message);
        }
        catch (UnsuccessfulResponseException e)
        {
            SnackbarService.EnqueueMessage(e.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }
    
    [RelayCommand]
    private void GoToSignUp() => NavigationService.NavigateTo<SignUpPage>();

    private async Task TryLogInAsync()
    {
        var dto = new LogInDto { Email = Email, Password = Password };
        
        ValidationHelper.LogInValidation(dto);
        
        var result = await ApiClient.LogInUserAsync(dto);

        if (result is false)
        {
            SnackbarService.EnqueueMessage("Należy uzupełnić dane użytkownika!");
            NavigationService.NavigateTo<SetUpUserCredentialsPage>();
            return;
        }
        
        SnackbarService.EnqueueMessage("Zalogowano pomyślnie!");
        
        // TODO: Navigate to home page
    }
}