using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Windows.Media.Effects;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GMap.NET;
using GMap.NET.WindowsPresentation;
using MaterialDesignThemes.Wpf;
using ZlecajGoApi;
using ZlecajGoApi.Dtos;
using ZlecajGoWpfApp.Services.Navigation;
using ZlecajGoWpfApp.Services.Snackbar;
using ZlecajGoWpfApp.Views;

namespace ZlecajGoWpfApp.ViewModels;

public partial class OffersViewModel : BaseViewModel
{
    public OffersViewModel(INavigationService navigationService, ISnackbarService snackbarService, IApiClient apiClient) 
        : base(navigationService, snackbarService, apiClient)
    {
        Title = "Zlecenia";
    }

    [ObservableProperty]
    private ObservableCollection<OfferDto> _offers = [];
    
    [ObservableProperty]
    private ObservableCollection<CategoryDto> _categories = [];
    
    [ObservableProperty]
    private ObservableCollection<StatusDto> _statuses = [];
    
    [ObservableProperty]
    private ObservableCollection<TypeDto> _types = [];
    
    [ObservableProperty]
    private ObservableCollection<GMapMarker> _mapOfferMarkers = [];
    
    [RelayCommand]
    private async Task LoadOffersAsync()
    {
        try
        {
            IsBusy = true;

            await FetchDataAsync(Categories, ApiClient.GetCategoriesAsync);
            await FetchDataAsync(Statuses, ApiClient.GetStatusesAsync);
            await FetchDataAsync(Types, ApiClient.GetTypesAsync);
            await FetchDataAsync(Offers, ApiClient.GetOffersAsync);

            foreach (var offer in Offers)
            {
                offer.CategoryName = Categories.First(c => c.Id == offer.CategoryId).Name;
                offer.StatusName = Statuses.First(s => s.Id == offer.StatusId).Name;
                offer.TypeName = Types.First(t => t.Id == offer.TypeId).Name;
            }
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

    [RelayCommand]
    private Task LoadMapOfferMarkersAsync()
    {
        try
        {
            IsBusy = true;

            if (MapOfferMarkers.Count != 0)
            {
                MapOfferMarkers.Clear();
            }

            foreach (var offer in Offers)
            {
                var marker = new GMapMarker(new PointLatLng(offer.Latitude, offer.Longitude))
                {
                    Shape = new PackIcon
                    {
                        Kind = PackIconKind.MapMarker,
                        Foreground = offer.TypeId == 1 ? Brushes.OrangeRed : Brushes.Gold,
                        Width = 48,
                        Height = 48,
                        ToolTip = $"{offer.TypeName}: {offer.Title}",
                        Effect = new DropShadowEffect
                        {
                            ShadowDepth = 0,
                            BlurRadius = 15,
                            Color = Colors.Black
                        }
                    }
                };
                MapOfferMarkers.Add(marker);
            }
        }
        catch (Exception e)
        {
            SnackbarService.EnqueueMessage(e.Message);
        }
        finally
        {
            IsBusy = false;
        }
        
        return Task.CompletedTask;
    }
    
    [RelayCommand]
    private void LogOut()
    {
        ApiClient.LogOutUser();
        NavigationService.NavigateTo<LogInPage>();
    }

    private async Task FetchDataAsync<T>(ObservableCollection<T> collection, Func<Task<List<T>>> fetchDataFunc)
    {
        var data = await fetchDataFunc();

        if (collection.Count != 0)
        {
            collection.Clear();
        }

        foreach (var item in data)
        {
            collection.Add(item);
        }
    }
}