using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
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
    internal static Window? UserAccountMenuContextWindow;
    internal static Frame? UserAccountMenuContextWindowFrame;
    
    [RelayCommand]
    private void NavigateToUserMenuNavigation() 
        => NavigationService.NavigateTo<UserAccountMenuNavigationPage>(UserAccountMenuContextWindow, UserAccountMenuContextWindowFrame!.Name);
    
#region UserDetailsCode
    [ObservableProperty]
    private UserDto _currentUser = UserSession.Instance.CurrentUser;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(EditModeDisabled))]
    private bool _editModeEnabled;
    public bool EditModeDisabled => !EditModeEnabled;

    [RelayCommand]
    private void NavigateToUserDetails() 
        => NavigationService.NavigateTo<UserDetailsPage>(UserAccountMenuContextWindow, UserAccountMenuContextWindowFrame!.Name);
    
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
#endregion

#region UserProvidedOffersCode
    [ObservableProperty]
    private static ObservableCollection<OfferDto> _userOffers = [];
    
    [ObservableProperty]
    private static ObservableCollection<TypeDto> _types = [];
    
    [ObservableProperty]
    private static ObservableCollection<CategoryDto> _categories = [];
    
    [ObservableProperty]
    private static ObservableCollection<StatusDto> _statuses = [];
    
    [ObservableProperty]
    private ICollectionView _userOffersView = CollectionViewSource.GetDefaultView(_userOffers);

    [ObservableProperty]
    private TypeDto? _selectedType;
    
    [ObservableProperty]
    private CategoryDto? _selectedCategory;
    
    [ObservableProperty]
    private StatusDto? _selectedStatus;
    
    [ObservableProperty]
    private string? _fromDate;
    
    [ObservableProperty]
    private string? _toDate;
    
    [RelayCommand]
    private void NavigateToUserOffers() 
        => NavigationService.NavigateTo<UserProvidedOffersPage>(UserAccountMenuContextWindow, UserAccountMenuContextWindowFrame!.Name);

    [RelayCommand]
    private void FilterOffers()
    {
        UserOffersView.Filter = o =>
        {
            var offer = (OfferDto)o;
            var isTypeMatch = SelectedType is null || SelectedType.Name == offer.TypeName;
            var isCategoryMatch = SelectedCategory is null || SelectedCategory.Name == offer.CategoryName;
            var isStatusMatch = SelectedStatus is null || SelectedStatus.Name == offer.StatusName;

            DateTime? fromDate = FromDate is null ? null : DateTime.Parse(FromDate, CultureInfo.InvariantCulture);
            DateTime? toDate = ToDate is null ? null : DateTime.Parse(ToDate, CultureInfo.InvariantCulture);
            var isDateMatch = (!fromDate.HasValue || offer.PostDateTime.Date >= fromDate.Value.Date) &&
                              (!toDate.HasValue || offer.PostDateTime.Date <= toDate.Value.Date);
            
            return isTypeMatch && isCategoryMatch && isStatusMatch && isDateMatch;
        };
    }

    [RelayCommand]
    private void ResetFilterOptions()
    {
        SelectedType = null;
        SelectedCategory = null;
        SelectedStatus = null;
        FromDate = null;
        ToDate = null;
        UserOffersView.Filter = null;
    }
#endregion
}