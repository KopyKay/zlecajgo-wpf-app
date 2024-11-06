using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ZlecajGoApi;
using ZlecajGoApi.Dtos;
using ZlecajGoApi.Exceptions;
using ZlecajGoWpfApp.Helpers;
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
    
    [ObservableProperty]
    private string _email = string.Empty;
    
    [ObservableProperty]
    private string _password = string.Empty;
    
    [ObservableProperty]
    private string _confirmPassword = string.Empty;

    [RelayCommand]
    private async Task SignUp()
    {
        try
        {
            IsBusy = true;
            await TrySignUpAsync();
        }
        catch (UnsuccessfulResponseException e)
        {
            SnackbarService.EnqueueMessage(e.Message);
        }
        catch (ArgumentException e)
        {
            SnackbarService.EnqueueMessage(e.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }
    
    [RelayCommand]
    private void GoToLogIn() => NavigationService.NavigateTo<LogInPage>();
    
    private async Task TrySignUpAsync()
    {
        var dto = new SignUpDto { Email = Email, Password = Password, ConfirmPassword = ConfirmPassword };
        ValidationHelper.SignUpValidation(dto);
        await ApiClient.SignUpUserAsync(dto);

        NavigationService.NavigateTo<SetUpUserCredentialsPage>();
    }
}