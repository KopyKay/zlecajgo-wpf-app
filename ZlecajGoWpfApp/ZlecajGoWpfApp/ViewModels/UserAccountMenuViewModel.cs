using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ZlecajGoApi;
using ZlecajGoApi.Dtos;
using ZlecajGoWpfApp.CustomControls;
using ZlecajGoWpfApp.Enums;
using ZlecajGoWpfApp.Helpers;
using ZlecajGoWpfApp.Services.Navigation;
using ZlecajGoWpfApp.Services.Snackbar;
using ZlecajGoWpfApp.Views;

namespace ZlecajGoWpfApp.ViewModels;

public partial class UserAccountMenuViewModel
(
    INavigationService navigationService,
    ISnackbarService snackbarService,
    IApiClient apiClient
)
: BaseViewModel(navigationService, snackbarService, apiClient)
{
    private const int PasswordMinLength = 6;
    
    internal static Window? UserAccountMenuContextWindow;
    internal static Frame? UserAccountMenuContextWindowFrame;
    
    [ObservableProperty]
    private UserDto _currentUser = UserSession.Instance.CurrentUser;
    
    [ObservableProperty]
    private ObservableCollection<OfferDto>? _userOffers;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(EditModeDisabled))]
    private bool _editModeEnabled;
    public bool EditModeDisabled => !EditModeEnabled;

    [ObservableProperty]
    private string? _newEmail;
    
    [ObservableProperty]
    [MinLength(3, ErrorMessage = ValidationHelper.FieldTooShortMessage)]
    [RegularExpression(ValidationHelper.UserNameRegex, ErrorMessage = ValidationHelper.FieldContainsIllegalCharactersMessage)]
    private string? _newUserName;
    partial void OnNewUserNameChanged(string? value) => SaveChangesCommand.NotifyCanExecuteChanged();

    [ObservableProperty]
    [RegularExpression(ValidationHelper.PhoneNumberRegex, ErrorMessage = ValidationHelper.IncorrectPhoneNumberMessage)]
    private string? _newPhoneNumber;
    partial void OnNewPhoneNumberChanged(string? value) => SaveChangesCommand.NotifyCanExecuteChanged();
    
    [ObservableProperty]
    [MinLength(PasswordMinLength, ErrorMessage = ValidationHelper.PasswordIsTooShortMessage)]
    [RegularExpression(ValidationHelper.PasswordRegex, ErrorMessage = ValidationHelper.PasswordDoesNotMeetRequirementsMessage)]
    private string? _newPassword;
    partial void OnNewPasswordChanged(string? value) => SaveChangesCommand.NotifyCanExecuteChanged();

    [ObservableProperty]
    [ValidationHelper.ComparePasswords(nameof(NewPassword))]
    private string? _confirmNewPassword;
    partial void OnConfirmNewPasswordChanged(string? value) => SaveChangesCommand.NotifyCanExecuteChanged();

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
    
    private void ClearFields()
    {
        NewEmail = null;
        NewUserName = null;
        NewPhoneNumber = null;
        NewPassword = null;
        ConfirmNewPassword = null;
    }
    
    private bool CanSaveChanges()
    {
        var areAnyFieldsFilled = !string.IsNullOrWhiteSpace(NewUserName) || !string.IsNullOrWhiteSpace(NewPhoneNumber);
        var arePasswordsFilled = !string.IsNullOrWhiteSpace(NewPassword) && !string.IsNullOrWhiteSpace(ConfirmNewPassword);

        return areAnyFieldsFilled || arePasswordsFilled;
    }
    
    [RelayCommand(CanExecute = nameof(CanSaveChanges))]
    private async Task SaveChanges()
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
                
                await ApiClient.UpdateUserCredentialsAsync(updateUserCredentialsDto);
            }
            
            if (!string.IsNullOrWhiteSpace(NewPassword) && !string.IsNullOrWhiteSpace(ConfirmNewPassword))
            {
                var changeUserPasswordDto = new ChangeUserPasswordDto
                {
                    NewPassword = NewPassword!,
                    ConfirmNewPassword = ConfirmNewPassword!,
                    CurrentPassword = confirmPasswordDialog.EnteredPassword
                };
                
                await ApiClient.ChangeUserPassword(changeUserPasswordDto);
                
                passwordChanged = true;
            }

            RefreshUser();
        }
        catch (ArgumentException e)
        {
            CustomMessageBox.Show(e.Message, CustomMessageBoxType.Error, "Błąd");
        }
        catch (Exception)
        {
            CustomMessageBox.Show("Wystąpił błąd podczas aktualizacji danych użytkownika", 
                CustomMessageBoxType.Error, "Błąd");
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
                
            CloseWindow();
            ApiClient.LogOutUser();
            NavigationService.NavigateTo<LogInPage>();
        }
    }
    
    [RelayCommand]
    private void NavigateToUserMenuNavigation() 
        => NavigationService.NavigateTo<UserAccountMenuNavigationPage>(UserAccountMenuContextWindow, UserAccountMenuContextWindowFrame!.Name);

    [RelayCommand]
    private void NavigateToUserOffers() 
        => NavigationService.NavigateTo<UserProvidedOffersPage>(UserAccountMenuContextWindow, UserAccountMenuContextWindowFrame!.Name);

    [RelayCommand]
    private void NavigateToUserDetails() 
        => NavigationService.NavigateTo<UserDetailsPage>(UserAccountMenuContextWindow, UserAccountMenuContextWindowFrame!.Name);

    [RelayCommand]
    private void CloseWindow() => UserAccountMenuContextWindow!.Close();

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