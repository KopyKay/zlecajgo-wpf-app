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