using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ZlecajGoApi;
using ZlecajGoApi.Dtos;
using ZlecajGoApi.Exceptions;
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
    
    [ObservableProperty]
    [Required(ErrorMessage = ValidationHelper.ErrorMessage.FieldIsRequired)]
    [EmailAddress(ErrorMessage = ValidationHelper.ErrorMessage.IncorrectEmail)]
    private string _email = string.Empty;
    
    [ObservableProperty]
    [Required(ErrorMessage = ValidationHelper.ErrorMessage.FieldIsRequired)]
    [MinLength(ValidationHelper.Constraint.PasswordMinLength, ErrorMessage = ValidationHelper.ErrorMessage.PasswordIsTooShort)]
    [RegularExpression(ValidationHelper.RegularExpression.Password, ErrorMessage = ValidationHelper.ErrorMessage.PasswordDoesNotMeetRequirements)]
    private string _password = string.Empty;
    partial void OnPasswordChanged(string value) => ValidatePassword();

    [ObservableProperty]
    [Required(ErrorMessage = ValidationHelper.ErrorMessage.FieldIsRequired)]
    [ValidationHelper.ComparePasswords(nameof(Password))]
    private string _confirmPassword = string.Empty;
    partial void OnConfirmPasswordChanged(string value) => ValidatePassword();

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
        ValidateAllProperties();

        if (HasErrors) return;
        
        try
        {
            IsBusy = true;
        
            var dto = new SignUpDto
            {
                Email = Email, 
                Password = Password, 
                ConfirmPassword = ConfirmPassword
            };
        
            await ApiClient.RegisterAsync(dto);

            NavigationService.NavigateTo<SetUpUserCredentialsPage>();
        }
        catch (EmailAlreadyInUseException e)
        {
            SnackbarService.EnqueueMessage(e.Message);
        }
        catch (Exception)
        {
            SnackbarService.EnqueueMessage(DefaultErrorMessage);
        }
        finally
        {
            IsBusy = false;
        }
    }
    
    [RelayCommand]
    private void GoToLogInPage() => NavigationService.NavigateTo<LogInPage>();
    
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