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
    private string _firstName = string.Empty;
    
    [ObservableProperty]
    private string _lastName = string.Empty;
    
    [ObservableProperty]
    private string _userName = string.Empty;

    [ObservableProperty]
    private string _birthDate = string.Empty;
    
    [ObservableProperty]
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
    
    private async Task TryUpdateUserCredentialsAsync()
    {
        var fullName = $"{FirstName} {LastName}";
        DateOnly birthDate = default;
        const string phoneNumberPrefix = "+48";
        var formatedPhoneNumber = string.Concat(PhoneNumber.Where(c => !char.IsWhiteSpace(c)));
        var phoneNumber = $"{phoneNumberPrefix}{formatedPhoneNumber}";
        
        if (!string.IsNullOrWhiteSpace(BirthDate))
        {
            var birthDateTime = DateTime
                .ParseExact(BirthDate, "M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture);
            birthDate = DateOnly.FromDateTime(birthDateTime);
        }
        
        var dto = new UpdateUserCredentialsDto
        {
            FullName = fullName,
            UserName = UserName,
            BirthDate = birthDate,
            PhoneNumber = phoneNumber
        };
        
        ValidationHelper.UpdateUserCredentialsValidation(dto);
        
        await ApiClient.UpdateUserCredentialsAsync(dto);
        
        SnackbarService.EnqueueMessage("Dane użytkownika zostały zaktualizowane!");
        
        NavigationService.NavigateTo<OffersPage>();
    }
}