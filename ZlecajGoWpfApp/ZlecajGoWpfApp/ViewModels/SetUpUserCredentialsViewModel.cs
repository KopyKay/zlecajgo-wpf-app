using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ZlecajGoApi;
using ZlecajGoApi.Dtos;
using ZlecajGoApi.Exceptions;
using ZlecajGoWpfApp.Helpers;
using ZlecajGoWpfApp.Services;

namespace ZlecajGoWpfApp.ViewModels;

public partial class SetUpUserCredentialsViewModel : BaseViewModel
{
    public SetUpUserCredentialsViewModel(NavigationService navigationService, SnackbarService snackbarService, IApiClient apiClient) 
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
        catch (InvalidOperationException e)
        {
            SnackbarService.EnqueueMessage(e.Message);
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
    
    private async Task TryUpdateUserCredentialsAsync()
    {
        var birthDateTime = DateTime
            .ParseExact(BirthDate, "M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture);
        const string phoneNumberPrefix = "+48";
        var formatedPhoneNumber = string.Concat(PhoneNumber.Where(c => !char.IsWhiteSpace(c)));
        
        var fullName = $"{FirstName} {LastName}";
        var birthDate = DateOnly.FromDateTime(birthDateTime);
        var phoneNumber = $"{phoneNumberPrefix}{formatedPhoneNumber}";
        
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
    }
}