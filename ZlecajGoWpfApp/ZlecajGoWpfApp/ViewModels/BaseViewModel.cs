using System.Collections.ObjectModel;
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
: ObservableValidator
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
    
    protected const string DefaultErrorMessage = "Wystąpił błąd po stronie serwera. Spróbuj ponownie później.";
    
    protected virtual async Task FetchDataAsync<T>(ObservableCollection<T> collection, Func<Task<List<T>?>> fetchDataFunc)
    {
        var data = await fetchDataFunc();

        if (data is null) return;
        
        if (collection.Count != 0)
        {
            collection.Clear();
        }

        foreach (var item in data)
        {
            collection.Add(item);
        }
    }

    protected virtual async Task ConnectToHubAsync<T>(Func<Task> connectFunc, Func<Task> connectFailFunc, Action? onConnected = null)
    {
        try
        {
            IsBusy = true;
            await connectFunc();
            onConnected?.Invoke();
        }
        catch (Exception)
        {
            await connectFailFunc();
        }
        finally
        {
            IsBusy = false;
        }
    }
}