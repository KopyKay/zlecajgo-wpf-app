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
using ZlecajGoWpfApp.Services.Navigation;
using ZlecajGoWpfApp.Services.Snackbar;
using ZlecajGoWpfApp.Views;

namespace ZlecajGoWpfApp.ViewModels;

public partial class OffersViewModel : BaseViewModel
{
    public OffersViewModel(INavigationService navigationService, ISnackbarService snackbarService, IApiClient apiClient,
        IServiceProvider serviceProvider) 
        : base(navigationService, snackbarService, apiClient)
    {
        _serviceProvider = serviceProvider;
        Title = "Zlecenia";
    }

    private readonly IServiceProvider _serviceProvider;
    
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
    private ObservableCollection<GMapMarker> _mapOfferMarkers = [];
    
    [ObservableProperty]
    private ObservableCollection<string> _availableCities = [];
    
    // TODO: Use materialDesign:HintAssist.Hint instead of declaring default options in string fields
    private const string DefaultTypeOption = "Wybierz typ";
    private const string DefaultCategoryOption = "Wybierz kategorię";
    private const string DefaultCityOption = "Wybierz miasto";
    
    [ObservableProperty]
    private string _selectedType = DefaultTypeOption;
    
    [ObservableProperty]
    private string _selectedCategory = DefaultCategoryOption;
    
    [ObservableProperty]
    private string _selectedCity = DefaultCityOption;
    
    [RelayCommand]
    private async Task LoadOffersAsync()
    {
        try
        {
            IsBusy = true;

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
        // TODO: Make ICollectionView for Markers
        // TODO: Markers should be only from available offers!
        try
        {
            IsBusy = true;

            if (MapOfferMarkers.Count != 0)
            {
                MapOfferMarkers.Clear();
            }

            var accentColorBrush1 = (Brush)Application.Current.Resources["AccentColorBrush1"]!;
            var accentColorBrush2 = (Brush)Application.Current.Resources["AccentColorBrush2"]!;
            
            foreach (var offer in Offers)
            {
                var marker = new GMapMarker(new PointLatLng(offer.Latitude, offer.Longitude))
                {
                    Shape = new PackIcon
                    {
                        Kind = PackIconKind.MapMarker,
                        Foreground = offer.TypeId == 1 ? accentColorBrush1 : accentColorBrush2,
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

    private void GetAvailableOffers()
    {
        AvailableOffersView = CollectionViewSource.GetDefaultView(Offers.Where(o => o.StatusId == 1));
    }
    
    private void GetAvailableCities()
    {
        if (AvailableOffersView is null) return;
        
        var uniqueCities = AvailableOffersView.Cast<OfferDto>()
            .Where(o => !string.IsNullOrWhiteSpace(o.City))
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

    [RelayCommand]
    private void FilterOffers()
    {
        if (AvailableOffersView is null) return;
        
        AvailableOffersView.Filter = o =>
        {
            var offer = (OfferDto)o;
            var isTypeMatch = SelectedType == DefaultTypeOption || offer.TypeName == SelectedType;
            var isCategoryMatch = SelectedCategory == DefaultCategoryOption || offer.CategoryName == SelectedCategory;
            var isCityMatch = SelectedCity == DefaultCityOption || offer.City == SelectedCity;

            return isTypeMatch && isCategoryMatch && isCityMatch;
        };
    }
    
    [RelayCommand]
    private void ResetFilterOptions()
    {
        SelectedType = DefaultTypeOption;
        SelectedCategory = DefaultCategoryOption;
        SelectedCity = DefaultCityOption;
        
        if (AvailableOffersView is not null)
        {
            AvailableOffersView.Filter = null;
        }
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