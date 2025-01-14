using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Effects;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GMap.NET;
using GMap.NET.WindowsPresentation;
using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.DependencyInjection;
using ZlecajGoApi;
using ZlecajGoApi.Dtos;
using ZlecajGoWpfApp.CustomControls;
using ZlecajGoWpfApp.Enums;
using ZlecajGoWpfApp.Services.Map;
using ZlecajGoWpfApp.Services.Navigation;
using ZlecajGoWpfApp.Services.Snackbar;
using ZlecajGoWpfApp.Views;

namespace ZlecajGoWpfApp.ViewModels;

public partial class OffersViewModel : BaseViewModel
{
    public OffersViewModel(INavigationService navigationService, ISnackbarService snackbarService, IApiClient apiClient,
        IServiceProvider serviceProvider, IMapService mapService) 
        : base(navigationService, snackbarService, apiClient)
    {
        _serviceProvider = serviceProvider;
        _mapService = mapService;
        _mapControl = mapService.MapControl;
        
        Title = "Zlecenia";
    }

    private readonly Brush _accentColorBrush1 = (Brush)Application.Current.Resources["AccentColorBrush1"]!;
    private readonly Brush _accentColorBrush2 = (Brush)Application.Current.Resources["AccentColorBrush2"]!;
    
    private readonly IServiceProvider _serviceProvider;
    
    private readonly IMapService _mapService;
    
    [ObservableProperty]
    private GMapControl _mapControl;
    
    [ObservableProperty]
    private ICollectionView? _availableOffersView;
    
    [ObservableProperty]
    private ObservableCollection<OfferDto> _offers = [];
    
    [ObservableProperty]
    private ObservableCollection<CategoryDto> _categories = [];
    
    [ObservableProperty]
    private ObservableCollection<StatusDto> _statuses = [];
    
    [ObservableProperty]
    private ObservableCollection<TypeDto> _types = [];
    
    [ObservableProperty]
    private ObservableCollection<string> _availableCities = [];
    
    [ObservableProperty]
    private TypeDto? _selectedType;
    
    [ObservableProperty]
    private CategoryDto? _selectedCategory;
    
    [ObservableProperty]
    private string? _selectedCity;
    
    [RelayCommand]
    private async Task FilterOffers()
    {
        IsBusy = true;
        
        if (AvailableOffersView is null) return;
    
        AvailableOffersView.Filter = o =>
        {
            var offer = (OfferDto)o;
            var isTypeMatch = SelectedType is null || SelectedType.Name == offer.TypeName;
            var isCategoryMatch = SelectedCategory is null || SelectedCategory.Name == offer.CategoryName;
            var isCityMatch = string.IsNullOrEmpty(SelectedCity) || SelectedCity == offer.City;

            return isTypeMatch && isCategoryMatch && isCityMatch;
        };

        await LoadMapMarkersFromAvailableOffersAsync();
        
        IsBusy = false;
    }
    
    [RelayCommand]
    private async Task ResetFilterOptions()
    {
        IsBusy = true;
        
        SelectedType = null;
        SelectedCategory = null;
        SelectedCity = null;
        
        if (AvailableOffersView is not null)
        {
            AvailableOffersView.Filter = null;
        }

        await LoadMapMarkersFromAvailableOffersAsync();
        
        IsBusy = false;
    }
    
    [RelayCommand]
    private void OpenAddOfferForm()
    {
        var createOfferWindow = _serviceProvider.GetService<CreateOfferWindow>();

        if (createOfferWindow?.DataContext is not CreateOfferViewModel createOfferViewModel)
        {
            SnackbarService.EnqueueMessage("Nie udało się otworzyć okna dodawania oferty. Spróbuj ponownie później.");
            return;
        }
        
        createOfferViewModel.OfferTypes = Types;
        createOfferViewModel.OfferCategories = Categories;
        
        createOfferWindow.ShowDialog();
    }
    
    [RelayCommand]
    private void LogOut()
    {
        ApiClient.LogOutUser();
        NavigationService.NavigateTo<LogInPage>();
    }
    
    public async Task InitializeOffersAndMapAsync()
    {
        try
        {
            IsBusy = true;
            
            await LoadOffersAsync();
            await LoadMapMarkersFromAvailableOffersAsync();
        }
        catch (Exception)
        {
            LogOut();

            CustomMessageBox.Show("Wystąpił błąd podczas ładowania zleceń. Zostałeś wylogowany.",
                CustomMessageBoxType.Error, "Błąd");
        }
        finally
        {
            IsBusy = false;
        }
    }
    
    private async Task LoadOffersAsync()
    {
        await FetchDataAsync(Categories, ApiClient.GetCategoriesAsync!);
        await FetchDataAsync(Statuses, ApiClient.GetStatusesAsync!);
        await FetchDataAsync(Types, ApiClient.GetTypesAsync!);
        await FetchDataAsync(Offers, ApiClient.GetOffersAsync);

        if (Offers.Count != 0)
        {
            GetAvailableOffers();
            GetAvailableCities();
        
            foreach (var offer in Offers)
            {
                // Fulfill offer with additional data
                offer.CategoryName = Categories.First(c => c.Id == offer.CategoryId).Name;
                offer.StatusName = Statuses.First(s => s.Id == offer.StatusId).Name;
                offer.TypeName = Types.First(t => t.Id == offer.TypeId).Name;
            }
        }
        else
        {
            SnackbarService.EnqueueMessage("Nie znaleziono zleceń/usług.");
        }
    }
    
    private Task LoadMapMarkersFromAvailableOffersAsync()
    {
        if (AvailableOffersView is null)
        {
            return Task.CompletedTask;
        }
        
        if (MapControl.Markers.Count != 0)
        {
            MapControl.Markers.Clear();
        }
        
        var availableOffers = AvailableOffersView.Cast<OfferDto>();
        
        foreach (var offer in availableOffers)
        {
            AddMapMarker(offer);
        }
        
        return Task.CompletedTask;
    }
    
    private void AddMapMarker(OfferDto dto)
    {
        var marker = new GMapMarker(new PointLatLng(dto.Latitude, dto.Longitude))
        {
            Shape = new PackIcon
            {
                Kind = PackIconKind.MapMarker,
                Foreground = dto.TypeId == 1 ? _accentColorBrush1 : _accentColorBrush2,
                Width = 48,
                Height = 48,
                ToolTip = $"{dto.TypeName}: {dto.Title}",
                Effect = new DropShadowEffect
                {
                    ShadowDepth = 0,
                    BlurRadius = 15,
                    Color = Colors.Black
                }
            },
            Tag = dto
        };
        MapControl.Markers.Add(marker);
    }
    
    private void GetAvailableOffers()
    {
        var availableOffers = Offers
            .Where(o => 
                o.StatusId == 1 &&
                o.ProviderId != UserSession.Instance.CurrentUser.Id);
        
        AvailableOffersView = CollectionViewSource.GetDefaultView(availableOffers);
    }
    
    private void GetAvailableCities()
    {
        if (AvailableOffersView is null) return;
        
        var uniqueCities = AvailableOffersView.Cast<OfferDto>()
            .Select(o => o.City)
            .Distinct()
            .OrderBy(city => city)
            .ToList();

        AvailableCities.Clear();
        
        foreach (var city in uniqueCities)
        {
            AvailableCities.Add(city);
        }
    }
    
    private async Task FetchDataAsync<T>(ObservableCollection<T> collection, Func<Task<List<T>?>> fetchDataFunc)
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
}