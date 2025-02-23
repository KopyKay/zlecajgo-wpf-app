using System.ComponentModel.DataAnnotations;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ZlecajGoApi;
using ZlecajGoApi.Dtos;
using ZlecajGoWpfApp.Helpers;
using ZlecajGoWpfApp.Services.Navigation;
using ZlecajGoWpfApp.Services.Snackbar;
using ZlecajGoWpfApp.Views;

namespace ZlecajGoWpfApp.ViewModels;

public partial class SetUpUserCredentialsViewModel : BaseViewModel
{
    public SetUpUserCredentialsViewModel(INavigationService navigationService, ISnackbarService snackbarService, IApiClient apiClient) 
        : base(navigationService, snackbarService, apiClient)
    {
        Title = "Uzupełnianie danych użytkownika";
    }
    
    [ObservableProperty]
    [Required(ErrorMessage = ValidationHelper.ErrorMessage.FieldIsRequired)]
    [MinLength(ValidationHelper.Constraint.FirstNameMinLength, ErrorMessage = ValidationHelper.ErrorMessage.FieldIsTooShort)]
    [RegularExpression(ValidationHelper.RegularExpression.FirstName, ErrorMessage = ValidationHelper.ErrorMessage.FieldContainsIllegalCharacters)]
    private string _firstName = string.Empty;
    
    [ObservableProperty]
    [Required(ErrorMessage = ValidationHelper.ErrorMessage.FieldIsRequired)]
    [MinLength(ValidationHelper.Constraint.LastNameMinLength, ErrorMessage = ValidationHelper.ErrorMessage.FieldIsTooShort)]
    [RegularExpression(ValidationHelper.RegularExpression.LastName, ErrorMessage = ValidationHelper.ErrorMessage.FieldContainsIllegalCharacters)]
    private string _lastName = string.Empty;
    
    [ObservableProperty]
    [Required(ErrorMessage = ValidationHelper.ErrorMessage.FieldIsRequired)]
    [MinLength(ValidationHelper.Constraint.UserNameMinLength, ErrorMessage = ValidationHelper.ErrorMessage.FieldIsTooShort)]
    [RegularExpression(ValidationHelper.RegularExpression.UserName, ErrorMessage = ValidationHelper.ErrorMessage.FieldContainsIllegalCharacters)]
    private string _userName = string.Empty;

    [ObservableProperty]
    [Required(ErrorMessage = ValidationHelper.ErrorMessage.FieldIsRequired)]
    [ValidationHelper.MinimumAge(ValidationHelper.Constraint.UserMinAge)]
    private string _birthDate = string.Empty;
    
    [ObservableProperty]
    [Required(ErrorMessage = ValidationHelper.ErrorMessage.FieldIsRequired)]
    [RegularExpression(ValidationHelper.RegularExpression.PolishPhoneNumber, ErrorMessage = ValidationHelper.ErrorMessage.IncorrectPhoneNumber)]
    private string _phoneNumber = string.Empty;

    [RelayCommand]
    private async Task UpdateUserCredentialsAsync()
    {
        try
        {
            IsBusy = true;
            await TryUpdateUserCredentialsAsync();
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
    
    partial void OnFirstNameChanged(string value) => ValidateProperty(value, nameof(FirstName));
    partial void OnLastNameChanged(string value) => ValidateProperty(value, nameof(LastName));
    partial void OnUserNameChanged(string value) => ValidateProperty(value, nameof(UserName));
    partial void OnBirthDateChanged(string value) => ValidateProperty(value, nameof(BirthDate));

    private async Task TryUpdateUserCredentialsAsync()
    {
        ValidateAllProperties();

        if (HasErrors) return;
        
        var fullName = $"{FirstName} {LastName}";
        var birthDate = DateOnly.FromDateTime(
                        DateTime.ParseExact(BirthDate, "M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture));
        var phoneNumber = string.Concat(PhoneNumber.Where(c => !char.IsWhiteSpace(c)));
        
        var dto = new UpdateUserCredentialsDto
        {
            FullName = fullName,
            BirthDate = birthDate,
            UserName = UserName,
            PhoneNumber = phoneNumber
        };
        
        await ApiClient.UpdateUserCredentialsAsync(dto);
        
        SnackbarService.EnqueueMessage("Dane użytkownika zostały zaktualizowane!");
        
        NavigationService.NavigateTo<OffersPage>();
    }
}