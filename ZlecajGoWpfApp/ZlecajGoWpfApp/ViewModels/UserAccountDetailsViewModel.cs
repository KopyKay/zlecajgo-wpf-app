using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ZlecajGoApi;
using ZlecajGoApi.Dtos;
using ZlecajGoApi.Exceptions;
using ZlecajGoWpfApp.CustomControls;
using ZlecajGoWpfApp.Enums;
using ZlecajGoWpfApp.Helpers;
using ZlecajGoWpfApp.Services.Navigation;
using ZlecajGoWpfApp.Services.Snackbar;
using ZlecajGoWpfApp.Views;

namespace ZlecajGoWpfApp.ViewModels;

public partial class UserAccountDetailsViewModel
(
    INavigationService navigationService,
    ISnackbarService snackbarService,
    IApiClient apiClient
)
: BaseViewModel(navigationService, snackbarService, apiClient)
{
    internal static Window? ParentWindow;
    internal static Frame? ParentWindowFrame;
    
    [ObservableProperty]
    private UserDto _currentUser = UserSession.Instance.CurrentUser;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(EditModeDisabled))]
    private bool _editModeEnabled;
    public bool EditModeDisabled => !EditModeEnabled;
    
    [ObservableProperty]
    private string? _newEmail;
    
    [ObservableProperty]
    [MinLength(ValidationHelper.Constraint.UserNameMinLength, ErrorMessage = ValidationHelper.ErrorMessage.FieldIsTooShort)]
    [RegularExpression(ValidationHelper.RegularExpression.UserName, ErrorMessage = ValidationHelper.ErrorMessage.FieldContainsIllegalCharacters)]
    private string? _newUserName;
    partial void OnNewUserNameChanged(string? value) => SaveChangesCommand.NotifyCanExecuteChanged();

    [ObservableProperty]
    [RegularExpression(ValidationHelper.RegularExpression.PolishPhoneNumber, ErrorMessage = ValidationHelper.ErrorMessage.IncorrectPhoneNumber)]
    private string? _newPhoneNumber;
    partial void OnNewPhoneNumberChanged(string? value) => SaveChangesCommand.NotifyCanExecuteChanged();
    
    [ObservableProperty]
    [MinLength(ValidationHelper.Constraint.PasswordMinLength, ErrorMessage = ValidationHelper.ErrorMessage.PasswordIsTooShort)]
    [RegularExpression(ValidationHelper.RegularExpression.Password, ErrorMessage = ValidationHelper.ErrorMessage.PasswordDoesNotMeetRequirements)]
    private string? _newPassword;
    partial void OnNewPasswordChanged(string? value) => SaveChangesCommand.NotifyCanExecuteChanged();

    [ObservableProperty]
    [ValidationHelper.ComparePasswords(nameof(NewPassword))]
    private string? _confirmNewPassword;
    partial void OnConfirmNewPasswordChanged(string? value) => SaveChangesCommand.NotifyCanExecuteChanged();
    
    [RelayCommand]
    private void NavigateToUserAccountMenuNavigation() 
        => NavigationService.NavigateTo<UserAccountMenuNavigationPage>(ParentWindow, ParentWindowFrame);
    
    [RelayCommand]
    private void TurnOnEditMode()
    {
        EditModeEnabled = true;
    }

    [RelayCommand]
    private void TurnOffEditMode()
    {
        EditModeEnabled = false;
        ClearFields();
        ClearErrors();
    }
    
    [RelayCommand(CanExecute = nameof(CanSaveChanges))]
    private async Task SaveChangesAsync()
    {
        ValidateNotEmptyFields();

        if (HasErrors) return;
        
        ConfirmPasswordDialog? confirmPasswordDialog = null;
        var passwordChanged = false;
        
        try
        {
            IsBusy = true;

            confirmPasswordDialog = new ConfirmPasswordDialog(ApiClient);
            var dialogResult = confirmPasswordDialog.ShowDialog();

            if (dialogResult is false) return;
            
            if (!string.IsNullOrWhiteSpace(NewUserName) || !string.IsNullOrWhiteSpace(NewPhoneNumber))
            {
                var updateUserCredentialsDto = new UpdateUserCredentialsDto
                {
                    UserName = string.IsNullOrWhiteSpace(NewUserName) ? null : NewUserName,
                    PhoneNumber = string.IsNullOrWhiteSpace(NewPhoneNumber) ? null : NewPhoneNumber
                };
                
                await ApiClient.UpdateUserAsync(updateUserCredentialsDto);
            }
            
            if (!string.IsNullOrWhiteSpace(NewPassword) && !string.IsNullOrWhiteSpace(ConfirmNewPassword))
            {
                var changeUserPasswordDto = new ChangeUserPasswordDto
                {
                    NewPassword = NewPassword!,
                    ConfirmNewPassword = ConfirmNewPassword!,
                    CurrentPassword = confirmPasswordDialog.EnteredPassword
                };
                
                await ApiClient.ChangeUserPasswordAsync(changeUserPasswordDto);
                
                passwordChanged = true;
            }

            RefreshUser();
        }
        catch (Exception e) when (e is UsernameAlreadyInUseException or PhoneNumberAlreadyInUseException)
        {
            CustomMessageBox.Show(e.Message, CustomMessageBoxType.Error);
        }
        catch (Exception)
        {
            CustomMessageBox.Show(DefaultErrorMessage, CustomMessageBoxType.Error);
        }
        finally
        {
            confirmPasswordDialog = null;
            ClearFields();
            EditModeEnabled = false;
            IsBusy = false;
        }

        if (passwordChanged)
        {
            CustomMessageBox.Show("Hasło zostało zmienione, zaloguj się ponownie.",
                CustomMessageBoxType.Information, "Operacja zakończona pomyślnie");
                
            ParentWindow!.Close();
            ApiClient.LogOutUser();
            NavigationService.NavigateTo<LogInPage>();
        }
    }
    
    private bool CanSaveChanges()
    {
        var areAnyFieldsFilled = !string.IsNullOrWhiteSpace(NewUserName) || !string.IsNullOrWhiteSpace(NewPhoneNumber);
        var arePasswordsFilled = !string.IsNullOrWhiteSpace(NewPassword) && !string.IsNullOrWhiteSpace(ConfirmNewPassword);

        return areAnyFieldsFilled || arePasswordsFilled;
    }
    
    private void ClearFields()
    {
        NewEmail = null;
        NewUserName = null;
        NewPhoneNumber = null;
        NewPassword = null;
        ConfirmNewPassword = null;
    }

    private void ValidateIfNotEmpty(string? propertyValue, [CallerArgumentExpression("propertyValue")] string propertyName = "")
    {
        if (!string.IsNullOrWhiteSpace(propertyValue))
        {
            ValidateProperty(propertyValue, propertyName);
        }
    }

    private void ValidateNotEmptyFields()
    {
        ClearErrors();
        ValidateIfNotEmpty(NewUserName);
        ValidateIfNotEmpty(NewPhoneNumber);
        ValidateIfNotEmpty(NewPassword);
        ValidateIfNotEmpty(ConfirmNewPassword);
    }
    
    private void RefreshUser() => CurrentUser = UserSession.Instance.CurrentUser;
}