using System.Windows;
using System.Windows.Controls;
using ZlecajGoWpfApp.Services.Map;
using ZlecajGoWpfApp.Services.Navigation;
using ZlecajGoWpfApp.ViewModels;

namespace ZlecajGoWpfApp.Views;

public partial class OffersPage : Page
{
    private readonly OffersViewModel _viewModel;
    private readonly IMapService _mapService;
    private readonly INavigationService _navigationService;
    
    public OffersPage(OffersViewModel viewModel, IMapService mapService, INavigationService navigationService)
    {
        InitializeComponent();
        
        DataContext = viewModel;
        _viewModel = viewModel;
        _mapService = mapService;
        _navigationService = navigationService;
        
        _mapService.InitializeMap(Map);
        
        Loaded += OnLoaded;
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        try
        {
            await _viewModel.LoadOffersCommand.ExecuteAsync(null);
            await _viewModel.LoadMapOfferMarkersCommand.ExecuteAsync(null);
            
            _mapService.AddMarkers(Map, _viewModel.MapOfferMarkers);
        }
        catch (Exception ex)
        {
            _viewModel.LogOutCommand.Execute(null);
            _navigationService.NavigateTo<LogInPage>();
            
            MessageBox.Show("Wystąpił błąd podczas ładowania zleceń. Spróbuj ponownie później.",
                "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}