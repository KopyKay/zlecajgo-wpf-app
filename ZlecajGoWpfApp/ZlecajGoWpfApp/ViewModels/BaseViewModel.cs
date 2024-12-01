using CommunityToolkit.Mvvm.ComponentModel;
using ZlecajGoApi;
using ZlecajGoWpfApp.Services.Navigation;
using ZlecajGoWpfApp.Services.Snackbar;

namespace ZlecajGoWpfApp.ViewModels;

public abstract partial class BaseViewModel
(
    INavigationService navigationService,
    ISnackbarService snackbarService,
    IApiClient apiClient
)
: ObservableObject
{
    protected readonly INavigationService NavigationService = navigationService;
    protected readonly ISnackbarService SnackbarService = snackbarService;
    protected readonly IApiClient ApiClient = apiClient;
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    private bool _isBusy;
    
    [ObservableProperty]
    private string _title = string.Empty;
    
    public bool IsNotBusy => !IsBusy;
}