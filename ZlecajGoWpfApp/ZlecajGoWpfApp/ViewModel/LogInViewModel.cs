using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using ZlecajGoWpfApp.Services;
using ZlecajGoWpfApp.View;

namespace ZlecajGoWpfApp.ViewModel;

public partial class LogInViewModel : BaseViewModel
{
    private readonly NavigationService _navigationService;
    
    public LogInViewModel(NavigationService navigationService)
    {
        _navigationService = navigationService;
        Title = "Logowanie";
    }
    
    [RelayCommand]
    private void GoToSignUp()
    {
        var singUpPage = App.AppHost.Services.GetRequiredService<SignUpPage>();
        _navigationService.NavigateTo(singUpPage);
    }
}