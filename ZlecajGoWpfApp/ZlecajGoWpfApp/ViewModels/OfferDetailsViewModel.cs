using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ZlecajGoApi;
using ZlecajGoApi.Dtos;
using ZlecajGoWpfApp.CustomControls;
using ZlecajGoWpfApp.Enums;
using ZlecajGoWpfApp.Services.Navigation;
using ZlecajGoWpfApp.Services.Snackbar;

namespace ZlecajGoWpfApp.ViewModels;

public partial class OfferDetailsViewModel : BaseViewModel
{
    public OfferDetailsViewModel(INavigationService navigationService, ISnackbarService snackbarService, IApiClient apiClient) 
        : base(navigationService, snackbarService, apiClient)
    {
        Title = "Szczegóły oferty";
    }

    [ObservableProperty]
    private OfferDto _offer = null!;

    [RelayCommand]
    private void ReportInterestToProvider()
    {
        // TODO: Implement this method
        CustomMessageBox.Show("Ten przycisk nie ma jeszcze implementacji.", CustomMessageBoxType.Warning, "Brak implementacji");
    }
    
    [RelayCommand]
    private void CloseWindow(Window window) => window.Close();
}