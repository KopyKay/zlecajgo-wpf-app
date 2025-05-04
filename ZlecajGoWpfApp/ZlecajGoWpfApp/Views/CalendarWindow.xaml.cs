using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ZlecajGoApi;
using ZlecajGoApi.Dtos;
using ZlecajGoWpfApp.CustomControls;
using ZlecajGoWpfApp.Models;
using ZlecajGoWpfApp.ViewModels;

namespace ZlecajGoWpfApp.Views;

public partial class CalendarWindow : Window
{
    public CalendarWindow(CalendarViewModel viewModel)
    {
        InitializeComponent();
        
        DataContext = viewModel;
        
        CalendarViewModel.CalendarWindow = this;
        _currentUser = UserSession.Instance.CurrentUser;
        
        Loaded += async (s, e) =>
        {
            await viewModel.Initialize();
            _offers = viewModel.Offers;
            _users = viewModel.Users;
        };
    }

    private List<OfferDto> _offers;
    private List<UserDto> _users;
    private readonly UserDto _currentUser;
    
    private void Contracts_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        var border = (Border)sender;
        var calendarDay = (CalendarViewModel.CalendarDay)border.DataContext;

        ShowCalendarDayContractsDetailsDialog(calendarDay?.Contracts, true);
    }

    private void Requests_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        var border = (Border)sender;
        var calendarDay = (CalendarViewModel.CalendarDay)border.DataContext;
    
        ShowCalendarDayContractsDetailsDialog(calendarDay?.Requests);
    }

    private void Services_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        var border = (Border)sender;
        var calendarDay = (CalendarViewModel.CalendarDay)border.DataContext;
    
        ShowCalendarDayContractsDetailsDialog(calendarDay?.Services);
    }

    private void ShowCalendarDayContractsDetailsDialog(List<OfferContractorDto>? calendarDayContracts, bool isContractType = false)
    {
        if (calendarDayContracts is not { Count: > 0 }) return;
        
        var offerContracts = calendarDayContracts
            .Select(oc => new 
            { 
                Offer = _offers.FirstOrDefault(o => o.Id == oc.OfferId),
                Contract = oc
            })
            .Where(x => x.Offer != null)
            .ToList();
        
        if (offerContracts.Count <= 0) return;
        
        var details = offerContracts.Select(oc =>
        {
            var executor = _users.FirstOrDefault(u => u.Id == oc.Contract.ContractorId);
            var provider = _users.FirstOrDefault(u => u.Id == oc.Offer!.ProviderId);
            var isCurrentUserExecutor = oc.Contract.ContractorId == _currentUser.Id;
            var isCurrentUserProvider = oc.Offer!.ProviderId == _currentUser.Id;

            return new OfferContractDetails
            {
                TypeName = isContractType ? "Kontrakty" : oc.Offer!.TypeName,
                TypeId = isContractType ? 0 : oc.Offer.TypeId,
                ProviderFullName = isCurrentUserProvider ? "Ty" : oc.Offer.ProviderFullName,
                ProviderPhoneNumber = isCurrentUserProvider ? "" : (provider?.PhoneNumber ?? "Brak numeru"),
                ExecutorFullName = isCurrentUserExecutor ? "Ty" : executor?.FullName ?? "Nieznany",
                ExecutorPhoneNumber = isCurrentUserExecutor ? "" : (executor?.PhoneNumber ?? "Brak numeru"),
                Title = oc.Offer.Title,
                ZipCode = oc.Offer.ZipCode,
                City = oc.Offer.City,
                Street = oc.Offer.Street,
                StartDateTime = oc.Contract.StartDateTime
            };
        }).ToList();
        
        var dialog = new OfferContractDetailsDialog(details);
        dialog.ShowDialog();
    }
}