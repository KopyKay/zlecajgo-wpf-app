using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Windows;
using System.Windows.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ZlecajGoApi;
using ZlecajGoApi.Dtos;
using ZlecajGoWpfApp.Helpers;
using ZlecajGoWpfApp.Services.Navigation;
using ZlecajGoWpfApp.Services.PostalAddress;
using ZlecajGoWpfApp.Services.Snackbar;

namespace ZlecajGoWpfApp.ViewModels;

// This class lifecycle is transient, that's why it implements IDisposable to unsubscribe events from PostalAddressService which is a singleton
public partial class CreateOfferViewModel : BaseViewModel
{
    public CreateOfferViewModel(INavigationService navigationService, ISnackbarService snackbarService, IApiClient apiClient,
        PostalAddressService postalAddressService) 
        : base(navigationService, snackbarService, apiClient)
    {
        _postalAddressService = postalAddressService;
        
        Title = "Nowe zlecenie/usługa";
    }
    
    private readonly PostalAddressService _postalAddressService;
    
    private Dictionary<string, List<string>>? _places;
    
    public EventHandler? RequestWindowClose;

    private const int TitleMinLength = 10;
    private const int DescriptionMinLength = 10;
    
    [ObservableProperty]
    private ObservableCollection<TypeDto> _offerTypes = [];
    
    [ObservableProperty]
    [Required(ErrorMessage = ValidationHelper.FieldIsRequiredMessage)]
    private TypeDto? _selectedOfferType;
    
    [ObservableProperty]
    private ObservableCollection<CategoryDto> _offerCategories = [];
    
    [ObservableProperty]
    [Required(ErrorMessage = ValidationHelper.FieldIsRequiredMessage)]
    private CategoryDto? _selectedOfferCategory;
    
    [ObservableProperty]
    [Required(ErrorMessage = ValidationHelper.FieldIsRequiredMessage)]
    [MinLength(TitleMinLength, ErrorMessage = ValidationHelper.FieldTooShortMessage)]
    [RegularExpression(ValidationHelper.TitleRegex, ErrorMessage = ValidationHelper.FieldContainsIllegalCharactersMessage)]
    private string _offerTitle = string.Empty;
    
    [ObservableProperty]
    [Required(ErrorMessage = ValidationHelper.FieldIsRequiredMessage)]
    [MinLength(DescriptionMinLength, ErrorMessage = ValidationHelper.FieldTooShortMessage)]
    [RegularExpression(ValidationHelper.DescriptionRegex, ErrorMessage = ValidationHelper.FieldContainsIllegalCharactersMessage)]
    private string _offerDescription = string.Empty;
    
    [ObservableProperty]
    [Required(ErrorMessage = ValidationHelper.FieldIsRequiredMessage)]
    [RegularExpression(ValidationHelper.PostalCodeRegex, ErrorMessage = ValidationHelper.FieldIncorrectFormatMessage)]
    private string _postalCode = string.Empty;
    
    [ObservableProperty]
    private ICollectionView? _placesView;

    [ObservableProperty]
    [Required(ErrorMessage = ValidationHelper.FieldIsRequiredMessage)]
    private string? _selectedPlace;
    
    [ObservableProperty]
    private bool _placesFound;
    
    [ObservableProperty]
    [RegularExpression(ValidationHelper.StreetNameRegex, ErrorMessage = ValidationHelper.FieldContainsIllegalCharactersMessage)]
    private string _streetName = string.Empty;
    
    [ObservableProperty]
    [Required(ErrorMessage = ValidationHelper.FieldIsRequiredMessage)]
    [RegularExpression(ValidationHelper.StreetNumberRegex, ErrorMessage = ValidationHelper.FieldContainsIllegalCharactersMessage)]
    private string _streetNumber = string.Empty;
    
    [ObservableProperty]
    private int[] _durationInDays = [1, 2, 3, 4, 5, 6, 7];

    [ObservableProperty]
    [Required(ErrorMessage = ValidationHelper.FieldIsRequiredMessage)]
    private int? _selectedDurationInDays;
    
    [ObservableProperty]
    [Required(ErrorMessage = ValidationHelper.FieldIsRequiredMessage)]
    private string _offerPrice = string.Empty;

    partial void OnSelectedOfferTypeChanged(TypeDto? value) => AddOfferCommand.NotifyCanExecuteChanged();
    partial void OnSelectedOfferCategoryChanged(CategoryDto? value) => AddOfferCommand.NotifyCanExecuteChanged();
    partial void OnOfferTitleChanged(string value)
    {
        ValidateProperty(value, nameof(OfferTitle));
        AddOfferCommand.NotifyCanExecuteChanged();
    }
    partial void OnOfferDescriptionChanged(string value)
    {
        ValidateProperty(value, nameof(OfferDescription));
        AddOfferCommand.NotifyCanExecuteChanged();
    }
    partial void OnPostalCodeChanged(string value)
    {
        if (_places is null) return;

        PlacesView ??=
            CollectionViewSource.GetDefaultView(new ObservableCollection<string>(_places.SelectMany(p => p.Value).Distinct()));
        
        PlacesView.Filter = item =>
        {
            if (item is string place)
            {
                return _places.TryGetValue(value, out var places) && places.Contains(place);
            }
            return false;
        };

        PlacesView.Refresh();
        PlacesFound = _places.ContainsKey(value);
        
        if (PlacesFound && _places[value].Count == 1)
        {
            SelectedPlace = _places[value].First();
        }
        
        AddOfferCommand.NotifyCanExecuteChanged();
    }
    partial void OnSelectedPlaceChanged(string? value) => AddOfferCommand.NotifyCanExecuteChanged();
    partial void OnStreetNameChanged(string value) => ValidateProperty(value, nameof(StreetName));
    partial void OnStreetNumberChanged(string value)
    {
        ValidateProperty(value, nameof(StreetNumber));
        AddOfferCommand.NotifyCanExecuteChanged();
    }
    partial void OnSelectedDurationInDaysChanged(int? value) => AddOfferCommand.NotifyCanExecuteChanged();
    partial void OnOfferPriceChanged(string value) => AddOfferCommand.NotifyCanExecuteChanged();
    
    private bool CanAddOffer()
    {
        return SelectedOfferType is not null &&
               SelectedOfferCategory is not null &&
               !string.IsNullOrWhiteSpace(OfferTitle) &&
               !string.IsNullOrWhiteSpace(OfferDescription) &&
               !string.IsNullOrWhiteSpace(PostalCode) &&
               SelectedPlace is not null &&
               !string.IsNullOrWhiteSpace(StreetNumber) &&
               SelectedDurationInDays is not null &&
               !string.IsNullOrWhiteSpace(OfferPrice);
    }
    
    [RelayCommand(CanExecute = nameof(CanAddOffer))]
    private void AddOffer(Window window)
    {
        ValidateAllProperties();
        
        // TODO: Implement add offer logic
    }

    [RelayCommand]
    private void Cancel(Window window) => window.Close();
    
    public async Task LoadPostalAddressesAsync()
    {
        try
        {
            IsBusy = true;
            _places = await _postalAddressService.GetPostalAddressesAsync();
        }
        catch (Exception e)
        {
            SnackbarService.EnqueueMessage(e.Message);
            RequestWindowClose?.Invoke(this, EventArgs.Empty);
        }
        finally
        {
            IsBusy = false;
        }
    }
}