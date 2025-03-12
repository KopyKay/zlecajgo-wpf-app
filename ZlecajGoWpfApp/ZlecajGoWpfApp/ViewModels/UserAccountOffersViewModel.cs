using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ZlecajGoApi;
using ZlecajGoApi.Dtos;
using ZlecajGoWpfApp.CustomControls;
using ZlecajGoWpfApp.Enums;
using ZlecajGoWpfApp.Services.Navigation;
using ZlecajGoWpfApp.Services.Snackbar;
using ZlecajGoWpfApp.Views;

namespace ZlecajGoWpfApp.ViewModels;

public partial class UserAccountOffersViewModel
(
    INavigationService navigationService,
    ISnackbarService snackbarService,
    IApiClient apiClient
)
: BaseViewModel(navigationService, snackbarService, apiClient)
{
    internal static Window? ParentWindow;
    internal static Frame? ParentWindowFrame;
    
    [ObservableProperty]
    private static ObservableCollection<OfferDto> _userOffers = [];
    
    [ObservableProperty]
    private static ObservableCollection<TypeDto> _types = [];
    
    [ObservableProperty]
    private static ObservableCollection<CategoryDto> _categories = [];
    
    [ObservableProperty]
    private static ObservableCollection<StatusDto> _statuses = [];
    
    [ObservableProperty]
    private ICollectionView _userOffersView = CollectionViewSource.GetDefaultView(_userOffers);

    [ObservableProperty]
    private TypeDto? _selectedType;
    
    [ObservableProperty]
    private CategoryDto? _selectedCategory;
    
    [ObservableProperty]
    private StatusDto? _selectedStatus;
    
    [ObservableProperty]
    private string? _fromDate;
    
    [ObservableProperty]
    private string? _toDate;

    [RelayCommand]
    private void NavigateToUserAccountMenuNavigation() 
        => NavigationService.NavigateTo<UserAccountMenuNavigationPage>(ParentWindow, ParentWindowFrame);
    
    [RelayCommand]
    private void FilterOffers()
    {
        UserOffersView.Filter = o =>
        {
            var offer = (OfferDto)o;
            var isTypeMatch = SelectedType is null || SelectedType.Name == offer.TypeName;
            var isCategoryMatch = SelectedCategory is null || SelectedCategory.Name == offer.CategoryName;
            var isStatusMatch = SelectedStatus is null || SelectedStatus.Name == offer.StatusName;

            DateTime? fromDate = FromDate is null ? null : DateTime.Parse(FromDate, CultureInfo.InvariantCulture);
            DateTime? toDate = ToDate is null ? null : DateTime.Parse(ToDate, CultureInfo.InvariantCulture);
            var isDateMatch = (!fromDate.HasValue || offer.PostDateTime.Date >= fromDate.Value.Date) &&
                              (!toDate.HasValue || offer.PostDateTime.Date <= toDate.Value.Date);
            
            return isTypeMatch && isCategoryMatch && isStatusMatch && isDateMatch;
        };
    }

    [RelayCommand]
    private void ResetFilterOptions()
    {
        SelectedType = null;
        SelectedCategory = null;
        SelectedStatus = null;
        FromDate = null;
        ToDate = null;
        UserOffersView.Filter = null;
    }

    [RelayCommand]
    private async Task EditOffer(OfferDto offerDto)
    {
        EditOfferDialog? editOfferDialog = null;

        try
        {
            IsBusy = true;
            
            editOfferDialog = new EditOfferDialog(offerDto);
            var dialogResult = editOfferDialog.ShowDialog();

            if (dialogResult is false) return;
            
            var selectedDuration = editOfferDialog.SelectedDuration;
            var newOfferPrice = editOfferDialog.NewOfferPrice;
            var offerPostDateTime = offerDto.PostDateTime;
            
            var offerExpiryDateTime = selectedDuration != 0 ? offerPostDateTime.AddDays(selectedDuration) : offerDto.ExpiryDateTime;
            var offerPrice = !string.IsNullOrWhiteSpace(newOfferPrice) ? decimal.Parse(newOfferPrice) : offerDto.Price;

            if (offerExpiryDateTime == offerDto.ExpiryDateTime && offerPrice == offerDto.Price)
            {
                CustomMessageBox.Show("Nie dokonano żadnych zmian.", CustomMessageBoxType.Information, "Informacja");
                return;
            }
            
            offerDto.ExpiryDateTime = offerExpiryDateTime;
            offerDto.Price = offerPrice;
            
            await ApiClient.UpdateOfferAsync(offerDto);
            
            CustomMessageBox.Show("Pomyślnie zaktualizowano ofertę!", CustomMessageBoxType.Confirmation, "Sukces");
        }
        catch (Exception)
        {
            CustomMessageBox.Show("Wystąpił błąd podczas edytowania!", CustomMessageBoxType.Error, "Błąd");
        }
        finally
        {
            editOfferDialog = null;
            IsBusy = false;
        }
    }
    
    [RelayCommand]
    private async Task DeleteOffer(OfferDto offerDto)
    {
        try
        {
            IsBusy = true;
            
            await ApiClient.DeleteOfferAsync(offerDto);
            UserOffers.Remove(offerDto);
            
            CustomMessageBox.Show("Pomyślnie usunięto ofertę!", CustomMessageBoxType.Confirmation, "Sukces");
        }
        catch (Exception)
        {
            CustomMessageBox.Show("Wystąpił błąd podczas usuwania!", CustomMessageBoxType.Error, "Błąd");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private static void ShowOfferExpireDate(OfferDto offerDto)
    {
        var remainingTime = offerDto.ExpiryDateTime - DateTime.Now;
        var remainingDays = (int)Math.Floor(remainingTime.TotalDays);
        var formattedExpiryDate = offerDto.ExpiryDateTime.ToString("dd MMMM yyyy");

        var message = remainingDays switch
        {
            <= 0 => $"Oferta wygaśnie dziś, {formattedExpiryDate}.",
            1 => $"Oferta jest ważna do jutra, {formattedExpiryDate}.",
            _ => $"Oferta jest ważna do {formattedExpiryDate}.\nPozostało {remainingDays} dni."
        };

        CustomMessageBox.Show(message, CustomMessageBoxType.Information, "Data wygaśnięcia oferty");
    }
    
    public async Task GetUserOffersAsync()
    {
        try
        {
            IsBusy = true;

            await FetchDataAsync(UserOffers, ApiClient.GetUserOffersAsync);
            await FetchDataAsync(Types, ApiClient.GetTypesAsync!);
            await FetchDataAsync(Categories, ApiClient.GetCategoriesAsync!);
            await FetchDataAsync(Statuses, ApiClient.GetStatusesAsync!);
            
            Statuses = new ObservableCollection<StatusDto>(Statuses
                .Where(s => s.Id is (int)OfferStatus.Pending or (int)OfferStatus.Taken or (int)OfferStatus.Completed));
            
            if (UserOffers.Count != 0)
            {
                foreach (var offer in UserOffers)
                {
                    offer.TypeName = Types.First(t => t.Id == offer.TypeId).Name;
                    offer.CategoryName = Categories.First(c => c.Id == offer.CategoryId).Name;
                    offer.StatusName = Statuses.First(s => s.Id == offer.StatusId).Name;
                }
            }
        }
        catch (Exception)
        {
            ParentWindow!.Close();
            SnackbarService.EnqueueMessage("Wystąpił błąd podczas pobierania zleceń/usług.");
        }
        finally
        {
            IsBusy = false;
        }
    }
}