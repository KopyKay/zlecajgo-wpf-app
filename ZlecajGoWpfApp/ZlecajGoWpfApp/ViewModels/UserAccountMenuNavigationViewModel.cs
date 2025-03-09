using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.Input;
using ZlecajGoApi;
using ZlecajGoWpfApp.Services.Navigation;
using ZlecajGoWpfApp.Services.Snackbar;
using ZlecajGoWpfApp.Views;

namespace ZlecajGoWpfApp.ViewModels;

public partial class UserAccountMenuNavigationViewModel
(
    INavigationService navigationService, 
    ISnackbarService snackbarService, 
    IApiClient apiClient
) 
: BaseViewModel(navigationService, snackbarService, apiClient)
{
    internal static Window? UserAccountMenuContextWindow;
    internal static Frame? UserAccountMenuContextWindowFrame;
    
    [RelayCommand]
    private void NavigateToUserAccountMenuNavigation() 
        => NavigationService.NavigateTo<UserAccountMenuNavigationPage>(UserAccountMenuContextWindow, UserAccountMenuContextWindowFrame);
    
    [RelayCommand]
    private void NavigateToUserAccountOffers() 
        => NavigationService.NavigateTo<UserAccountOffersPage>(UserAccountMenuContextWindow, UserAccountMenuContextWindowFrame);
    
    [RelayCommand]
    private void NavigateToUserAccountDetails() 
        => NavigationService.NavigateTo<UserAccountDetailsPage>(UserAccountMenuContextWindow, UserAccountMenuContextWindowFrame);
    
    [RelayCommand]
    private static void CloseWindow() => UserAccountMenuContextWindow!.Close();
}