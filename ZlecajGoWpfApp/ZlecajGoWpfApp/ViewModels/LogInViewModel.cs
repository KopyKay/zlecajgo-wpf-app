using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ZlecajGoApi;
using ZlecajGoApi.Dtos;
using ZlecajGoWpfApp.Helpers;
using ZlecajGoWpfApp.Services.Navigation;
using ZlecajGoWpfApp.Services.Snackbar;
using ZlecajGoWpfApp.Views;
using UnauthorizedAccessException = ZlecajGoApi.Exceptions.UnauthorizedAccessException;

namespace ZlecajGoWpfApp.ViewModels;

public partial class LogInViewModel : BaseViewModel
{
    public LogInViewModel(INavigationService navigationService, ISnackbarService snackbarService, IApiClient apiClient) 
        : base(navigationService, snackbarService, apiClient)
    {
        Title = "Logowanie";
    }
    
    [ObservableProperty]
    [Required(ErrorMessage = ValidationHelper.ErrorMessage.FieldIsRequired)]
    [EmailAddress(ErrorMessage = ValidationHelper.ErrorMessage.IncorrectEmail)]
    private string _email = string.Empty;
    
    [ObservableProperty]
    [Required(ErrorMessage = ValidationHelper.ErrorMessage.FieldIsRequired)]
    private string _password = string.Empty;
    
    [RelayCommand]
    private async Task LogIn()
    {
        ValidateAllProperties();

        if (HasErrors) return;
        
        try
        {
            IsBusy = true;
        
            var dto = new LogInDto
            {
                Email = Email, 
                Password = Password
            };
        
            var result = await ApiClient.LoginAsync(dto);

            if (result is false)
            {
                SnackbarService.EnqueueMessage("Należy uzupełnić dane użytkownika!");
                NavigationService.NavigateTo<SetUpUserCredentialsPage>();
                return;
            }
        
            SnackbarService.EnqueueMessage("Zalogowano pomyślnie!");
            NavigationService.NavigateTo<OffersPage>();
        }
        catch (UnauthorizedAccessException e)
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
    private void GoToSignUpPage() => NavigationService.NavigateTo<SignUpPage>();
}