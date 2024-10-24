using CommunityToolkit.Mvvm.ComponentModel;
using MaterialDesignThemes.Wpf;
using ZlecajGoApi;
using ZlecajGoWpfApp.Services;

namespace ZlecajGoWpfApp.ViewModel;

public abstract partial class BaseViewModel
(
    NavigationService navigationService,
    SnackbarService snackbarService,
    IApiClient apiClient
)
: ObservableObject
{
    protected readonly NavigationService NavigationService = navigationService;
    protected readonly SnackbarService SnackbarService = snackbarService;
    protected readonly IApiClient ApiClient = apiClient;
    
    public SnackbarMessageQueue SnackbarMessageQueue => SnackbarService.MessageQueue;
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    private bool _isBusy;
    
    [ObservableProperty]
    private string _title = string.Empty;
    
    public bool IsNotBusy => !IsBusy;
}