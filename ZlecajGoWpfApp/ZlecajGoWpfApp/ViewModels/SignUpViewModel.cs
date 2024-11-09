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
    
    [ObservableProperty]
    private bool _hasLowerCase;
    
    [ObservableProperty]
    private bool _hasUpperCase;

    [ObservableProperty]
    private bool _hasNumber;

    [ObservableProperty]
    private bool _hasSpecialCharacter;

    [ObservableProperty]
    private bool _hasMinimumLength;

    [ObservableProperty]
    private bool _passwordsMatch;
    
    partial void OnPasswordChanged(string value)
    {
        ValidatePassword();
    }

    partial void OnConfirmPasswordChanged(string value)
    {
        ValidatePassword();
    }

    private void ValidatePassword()
    {
        HasLowerCase = Password.Any(char.IsLower);
        HasUpperCase = Password.Any(char.IsUpper);
        HasNumber = Password.Any(char.IsDigit);
        HasSpecialCharacter = Password.Any(ch => !char.IsLetterOrDigit(ch));
        HasMinimumLength = Password.Length >= 6;
        
        if (!string.IsNullOrEmpty(Password) && !string.IsNullOrEmpty(ConfirmPassword))
        {
            PasswordsMatch = Password == ConfirmPassword;
        }
        else
        {
            PasswordsMatch = false;
        }
    }

    [RelayCommand]
    private async Task SignUp()
    {
        try
        {
            IsBusy = true;
            await TrySignUpAsync();
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
    private void GoToLogIn() => NavigationService.NavigateTo<LogInPage>();
    
    private async Task TrySignUpAsync()
    {
        var dto = new SignUpDto { Email = Email, Password = Password, ConfirmPassword = ConfirmPassword };
        
        ValidationHelper.SignUpValidation(dto);
        
        await ApiClient.SignUpUserAsync(dto);

        NavigationService.NavigateTo<SetUpUserCredentialsPage>();
    }
}