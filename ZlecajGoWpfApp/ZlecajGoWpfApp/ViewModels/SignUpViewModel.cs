using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ZlecajGoApi;
using ZlecajGoApi.Dtos;
using ZlecajGoWpfApp.Helpers;
using ZlecajGoWpfApp.Services.Navigation;
using ZlecajGoWpfApp.Services.Snackbar;
using ZlecajGoWpfApp.Views;

namespace ZlecajGoWpfApp.ViewModels;

public partial class SignUpViewModel : BaseViewModel
{
    public SignUpViewModel(INavigationService navigationService, ISnackbarService snackbarService, IApiClient apiClient) 
        : base(navigationService, snackbarService, apiClient)
    {
        Title = "Rejestracja";
    }
    
    private const int PasswordMinLength = 6;
    
    [ObservableProperty]
    [Required(ErrorMessage = ValidationHelper.FieldIsRequiredMessage)]
    [EmailAddress(ErrorMessage = ValidationHelper.IncorrectEmailMessage)]
    private string _email = string.Empty;
    
    [ObservableProperty]
    [Required(ErrorMessage = ValidationHelper.FieldIsRequiredMessage)]
    [MinLength(PasswordMinLength, ErrorMessage = ValidationHelper.PasswordIsTooShortMessage)]
    [RegularExpression(ValidationHelper.PasswordRegex, ErrorMessage = ValidationHelper.PasswordDoesNotMeetRequirementsMessage)]
    private string _password = string.Empty;
    
    [ObservableProperty]
    [Required(ErrorMessage = ValidationHelper.FieldIsRequiredMessage)]
    [ValidationHelper.ComparePasswords(nameof(Password))]
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

    [RelayCommand]
    private async Task SignUp()
    {
        try
        {
            IsBusy = true;
            await TrySignUpAsync();
        }
        catch (Exception e)
        {
            SnackbarService.EnqueueMessage(e.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }
    
    [RelayCommand]
    private void GoToLogInPage() => NavigationService.NavigateTo<LogInPage>();
    
    private async Task TrySignUpAsync()
    {
        ValidateAllProperties();

        if (HasErrors) return;
        
        var dto = new SignUpDto
        {
            Email = Email, 
            Password = Password, 
            ConfirmPassword = ConfirmPassword
        };
        
        await ApiClient.SignUpUserAsync(dto);

        NavigationService.NavigateTo<SetUpUserCredentialsPage>();
    }
    
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
}