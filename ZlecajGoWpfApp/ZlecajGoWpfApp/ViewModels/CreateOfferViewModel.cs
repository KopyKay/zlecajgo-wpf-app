using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Windows;
using System.Windows.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ZlecajGoApi;
using ZlecajGoApi.Dtos;
using ZlecajGoApi.Exceptions;
using ZlecajGoWpfApp.CustomControls;
using ZlecajGoWpfApp.Enums;
using ZlecajGoWpfApp.Helpers;
using ZlecajGoWpfApp.Services.Map;
using ZlecajGoWpfApp.Services.Navigation;
using ZlecajGoWpfApp.Services.PostalAddress;
using ZlecajGoWpfApp.Services.Snackbar;

namespace ZlecajGoWpfApp.ViewModels;

public partial class CreateOfferViewModel : BaseViewModel
{
    public CreateOfferViewModel(INavigationService navigationService, ISnackbarService snackbarService, IApiClient apiClient,
        PostalAddressService postalAddressService, IMapService mapService) 
        : base(navigationService, snackbarService, apiClient)
    {
        _postalAddressService = postalAddressService;
        _mapService = mapService;
        
        Title = "Nowe zlecenie/usługa";
    }
    
    private readonly PostalAddressService _postalAddressService;
    
    private readonly IMapService _mapService;
    
    private Dictionary<string, List<string>>? _places;
    
    public EventHandler? RequestWindowClose;
    
    [ObservableProperty]
    private ObservableCollection<TypeDto> _offerTypes = [];
    
    [ObservableProperty]
    [Required(ErrorMessage = ValidationHelper.ErrorMessage.FieldIsRequired)]
    private TypeDto? _selectedOfferType;
    partial void OnSelectedOfferTypeChanged(TypeDto? value) => AddOfferCommand.NotifyCanExecuteChanged();
    
    [ObservableProperty]
    private ObservableCollection<CategoryDto> _offerCategories = [];
    
    [ObservableProperty]
    [Required(ErrorMessage = ValidationHelper.ErrorMessage.FieldIsRequired)]
    private CategoryDto? _selectedOfferCategory;
    partial void OnSelectedOfferCategoryChanged(CategoryDto? value) => AddOfferCommand.NotifyCanExecuteChanged();
    
    [ObservableProperty]
    [Required(ErrorMessage = ValidationHelper.ErrorMessage.FieldIsRequired)]
    [MinLength(ValidationHelper.Constraint.OfferTitleMinLength, ErrorMessage = ValidationHelper.ErrorMessage.FieldIsTooShort)]
    [RegularExpression(ValidationHelper.RegularExpression.OfferTitle, ErrorMessage = ValidationHelper.ErrorMessage.FieldContainsIllegalCharacters)]
    private string _offerTitle = string.Empty;
    partial void OnOfferTitleChanged(string value)
    {
        ValidateProperty(value, nameof(OfferTitle));
        AddOfferCommand.NotifyCanExecuteChanged();
    }
    
    [ObservableProperty]
    [Required(ErrorMessage = ValidationHelper.ErrorMessage.FieldIsRequired)]
    [MinLength(ValidationHelper.Constraint.OfferDescriptionMinLength, ErrorMessage = ValidationHelper.ErrorMessage.FieldIsTooShort)]
    [RegularExpression(ValidationHelper.RegularExpression.OfferDescription, ErrorMessage = ValidationHelper.ErrorMessage.FieldContainsIllegalCharacters)]
    private string _offerDescription = string.Empty;
    partial void OnOfferDescriptionChanged(string value)
    {
        ValidateProperty(value, nameof(OfferDescription));
        AddOfferCommand.NotifyCanExecuteChanged();
    }

    [ObservableProperty]
    [Required(ErrorMessage = ValidationHelper.ErrorMessage.FieldIsRequired)]
    [RegularExpression(ValidationHelper.RegularExpression.PostalCode, ErrorMessage = ValidationHelper.ErrorMessage.FieldIncorrectFormat)]
    private string _postalCode = string.Empty;
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

    [ObservableProperty]
    private ICollectionView? _placesView;

    [ObservableProperty]
    [Required(ErrorMessage = ValidationHelper.ErrorMessage.FieldIsRequired)]
    private string? _selectedPlace;
    partial void OnSelectedPlaceChanged(string? value) => AddOfferCommand.NotifyCanExecuteChanged();

    [ObservableProperty]
    private bool _placesFound;
    
    [ObservableProperty]
    [RegularExpression(ValidationHelper.RegularExpression.StreetName, ErrorMessage = ValidationHelper.ErrorMessage.FieldContainsIllegalCharacters)]
    private string _streetName = string.Empty;
    partial void OnStreetNameChanged(string value) => ValidateProperty(value, nameof(StreetName));

    [ObservableProperty]
    [Required(ErrorMessage = ValidationHelper.ErrorMessage.FieldIsRequired)]
    [RegularExpression(ValidationHelper.RegularExpression.StreetNumber, ErrorMessage = ValidationHelper.ErrorMessage.FieldContainsIllegalCharacters)]
    private string _streetNumber = string.Empty;
    partial void OnStreetNumberChanged(string value)
    {
        ValidateProperty(value, nameof(StreetNumber));
        AddOfferCommand.NotifyCanExecuteChanged();
    }

    [ObservableProperty]
    private int[] _durationInDays = [1, 2, 3, 4, 5, 6, 7];

    [ObservableProperty]
    [Required(ErrorMessage = ValidationHelper.ErrorMessage.FieldIsRequired)]
    private int? _selectedDurationInDays;
    partial void OnSelectedDurationInDaysChanged(int? value) => AddOfferCommand.NotifyCanExecuteChanged();

    [ObservableProperty]
    [Required(ErrorMessage = ValidationHelper.ErrorMessage.FieldIsRequired)]
    private string _offerPrice = string.Empty;
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
    private async Task AddOffer(Window window)
    {
        ValidateAllProperties();
        
        if (HasErrors) return;
        
        var street = string.IsNullOrWhiteSpace(StreetName) ? StreetNumber : $"{StreetName} {StreetNumber}";

        try
        {
            IsBusy = true;
            
            var coordinates = await _mapService.GetCoordinates(PostalCode, SelectedPlace!, street);
            
            var dto = new OfferDto
            {
                Title = OfferTitle,
                Description = OfferDescription,
                Price = decimal.Parse(OfferPrice),
                ExpiryDateTime = DateTime.Now.AddDays(SelectedDurationInDays!.Value),
                City = SelectedPlace!,
                Street = street,
                ZipCode = PostalCode,
                Latitude = coordinates.lat,
                Longitude = coordinates.lon,
                TypeId = SelectedOfferType!.Id,
                CategoryId = SelectedOfferCategory!.Id
            };
            
            await ApiClient.CreateOfferAsync(dto);
            
            RequestWindowClose?.Invoke(this, EventArgs.Empty);
            
            var typeName = SelectedOfferType.Id == (int)OfferType.Request ? "zlecenie" : "usługę";
            SnackbarService.EnqueueMessage($"Pomyślnie dodano {typeName}!");
        }
        catch (InvalidAddressException e)
        {
            CustomMessageBox.Show(e.Message, CustomMessageBoxType.Warning);
        }
        catch (CoordinatesNotFoundException e)
        {
            CustomMessageBox.Show(e.Message, CustomMessageBoxType.Error);
        }
        catch (Exception)
        {
            CustomMessageBox.Show("Wystąpił problem po stronie serwera, spróbuj ponownie później.", CustomMessageBoxType.Error);
        }
        finally
        {
            IsBusy = false;
        }
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
            RequestWindowClose?.Invoke(this, EventArgs.Empty);
            CustomMessageBox.Show(e.Message, CustomMessageBoxType.Error);
        }
        finally
        {
            IsBusy = false;
        }
    }
}