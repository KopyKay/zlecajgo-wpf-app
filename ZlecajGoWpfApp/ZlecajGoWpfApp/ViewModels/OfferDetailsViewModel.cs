using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using ZlecajGoApi;
using ZlecajGoApi.Dtos;
using ZlecajGoApi.Hubs.Chat;
using ZlecajGoWpfApp.CustomControls;
using ZlecajGoWpfApp.Enums;
using ZlecajGoWpfApp.Views;
using ZlecajGoWpfApp.Services.Navigation;
using ZlecajGoWpfApp.Services.Snackbar;

namespace ZlecajGoWpfApp.ViewModels;

public partial class OfferDetailsViewModel : BaseViewModel
{
    public OfferDetailsViewModel(INavigationService navigationService, ISnackbarService snackbarService, IApiClient apiClient,
        IServiceProvider serviceProvider, IChatHubClient chatHubClient) : base(navigationService, snackbarService, apiClient)
    {
        Title = "Szczegóły oferty";
        
        _chatHubClient = chatHubClient;
        _serviceProvider = serviceProvider;
    }

    internal static Window? OfferDetailsWindow;
    
    private readonly IChatHubClient _chatHubClient;
    private readonly IServiceProvider _serviceProvider;
    
    [ObservableProperty]
    private OfferDto _offer = null!;

    [RelayCommand]
    private async Task ReportInterestToProvider()
    {
        const string messagePart1 = "Jestem zainteresowany Pańską ofertą:";
        const string messagePart2 = "Czy jest aktualna?";
        var initialMessage = $"{messagePart1}\n\n• {Offer.Title}\n\n{messagePart2}";
        
        try
        {
            IsBusy = true;
            
            await _chatHubClient.CreateChatAsync(Offer.ProviderId, initialMessage);
            var chatWindow = _serviceProvider.GetService<ChatWindow>()!;
            CloseWindow();
            chatWindow.ShowDialog();
        }
        catch (Exception)
        {
            CustomMessageBox.Show("Wystąpił błąd! Spróbuj ponownie później.", CustomMessageBoxType.Error, "Błąd");
        }
        finally
        {
            IsBusy = false;
        }
    }
    
    [RelayCommand]
    private static void CloseWindow() => OfferDetailsWindow!.Close();
}