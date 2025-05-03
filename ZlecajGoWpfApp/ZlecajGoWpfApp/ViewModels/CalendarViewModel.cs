using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ZlecajGoApi;
using ZlecajGoApi.Dtos;
using ZlecajGoWpfApp.Enums;
using ZlecajGoWpfApp.Services.Navigation;
using ZlecajGoWpfApp.Services.Snackbar;

namespace ZlecajGoWpfApp.ViewModels;

public partial class CalendarViewModel : BaseViewModel
{
    public CalendarViewModel(INavigationService navigationService, ISnackbarService snackbarService, IApiClient apiClient) 
        : base(navigationService, snackbarService, apiClient)
    {
        Title = "Kalendarz";
    }
    
    internal static Window? CalendarWindow;
    private readonly CultureInfo _polishCultureInfo = new("pl-PL");
    private List<OfferDto> _offers = [];
    
    [ObservableProperty]
    private ObservableCollection<OfferContractorDto> _providedOffers = [];
    
    [ObservableProperty]
    private ObservableCollection<OfferContractorDto> _contractedOffers = [];
    
    [ObservableProperty]
    private ObservableCollection<CalendarDay> _days = [];
    
    [ObservableProperty]
    private List<string>? _months;
    
    [ObservableProperty]
    private List<string>? _years;
    
    [ObservableProperty]
    private string? _selectedMonth;

    [ObservableProperty]
    private string? _selectedYear;

    [RelayCommand]
    private void GoToNextMonth()
    {
        var currentMonthIndex = Months!.IndexOf(SelectedMonth!);
        if (currentMonthIndex == 11)
        {
            SelectedMonth = Months[0];
            var currentYear = int.Parse(SelectedYear!);
            var nextYear = (currentYear + 1).ToString();
        
            if (Years!.Contains(nextYear)) SelectedYear = nextYear;
        }
        else
        {
            SelectedMonth = Months[currentMonthIndex + 1];
        }
    }
    
    [RelayCommand]
    private void GoToPreviousMonth()
    {
        var currentMonthIndex = Months!.IndexOf(SelectedMonth!);
        if (currentMonthIndex == 0)
        {
            SelectedMonth = Months[11];
            var currentYear = int.Parse(SelectedYear!);
            var prevYear = (currentYear - 1).ToString();
        
            if (Years!.Contains(prevYear)) SelectedYear = prevYear;
        }
        else
        {
            SelectedMonth = Months[currentMonthIndex - 1];
        }
    }
    
    [RelayCommand]
    private static void CloseWindow() => CalendarWindow!.Close();

    partial void OnSelectedMonthChanged(string? value)
    {
        UpdateCalendarDays();
    }

    partial void OnSelectedYearChanged(string? value)
    {
        UpdateCalendarDays();
    }

    private void UpdateCalendarDays()
    {
        if (SelectedMonth == null || SelectedYear == null || Months == null || Years == null)
        {
            return;
        }
        
        Days.Clear();
        
        var month = Months!.IndexOf(SelectedMonth!) + 1;
        var year = int.Parse(SelectedYear!);
        var firstDayOfMonth = new DateTime(year, month, 1);
        
        // Get day of the week (1=Monday, 7=Sunday in Polish culture)
        var dayOfWeek = (int)_polishCultureInfo.Calendar.GetDayOfWeek(firstDayOfMonth);
        if (dayOfWeek == 0) dayOfWeek = 7; // Sunday should be 7, not 0
        
        // Calculate the start date - may be from the previous month
        var startDate = firstDayOfMonth.AddDays(-(dayOfWeek - 1));

        // Generate 42 days (6 rows x 7 columns) for a calendar grid
        for (var i = 0; i < 42; i++)
        {
            var currentDate = startDate.AddDays(i);
            var day = new CalendarDay
            {
                Day = currentDate.Day,
                Date = currentDate,
                IsCurrentMonth = currentDate.Month == month && currentDate.Year == year,
                IsToday = currentDate.Date == DateTime.Today
            };

            var contracts = ProvidedOffers
                .Where(oc => oc.StartDateTime.Date == currentDate.Date)
                .Where(oc => _offers.Any(o => o.Id == oc.OfferId && o.TypeId != (int)OfferType.Service))
                .ToList();

            var requests = ContractedOffers
                .Where(oc => oc.StartDateTime.Date == currentDate.Date)
                .ToList();

            var services = ProvidedOffers
                .Where(oc => oc.StartDateTime.Date == currentDate.Date)
                .Where(oc => _offers.Any(o => o.Id == oc.OfferId && o.TypeId == (int)OfferType.Service))
                .ToList();

            day.HasContractor = contracts.Count > 0;
            day.HasRequest = requests.Count > 0;
            day.HasService = services.Count > 0;
            day.Contracts = contracts;
            day.Requests = requests;
            day.Services = services;

            Days.Add(day);
        }
    }
    
    public async Task Initialize()
    {
        try
        {
            IsBusy = true;

            Months = Enumerable.Range(1, 12)
                .Select(month =>
                {
                    var monthName = _polishCultureInfo.DateTimeFormat.GetMonthName(month);
                    return char.ToUpper(monthName[0]) + monthName[1..];
                })
                .ToList();

            var currentYear = DateTime.Now.Year;
            Years = Enumerable.Range(currentYear - 5, 11)
                .Select(year => year.ToString())
                .ToList();

            SelectedMonth = Months[DateTime.Now.Month - 1];
            SelectedYear = currentYear.ToString();

            _offers = (await ApiClient.GetOffersAsync())!;
            await FetchDataAsync(ProvidedOffers, ApiClient.GetProvidedOffersWithContractorAsync);
            await FetchDataAsync(ContractedOffers, ApiClient.GetContractedOffersAsync);
            UpdateCalendarDays();
        }
        catch (Exception)
        {
            CloseWindow();
            SnackbarService.EnqueueMessage("Błąd podczas ładowania kalendarza! Spróbuj ponownie później.");
        }
        finally
        {
            IsBusy = false;
        }
    }

    public class CalendarDay
    {
        public int Day { get; set; }
        public DateTime Date { get; set; }
        public bool IsCurrentMonth { get; set; }
        public bool IsToday { get; set; }
        public bool IsPastDate => Date < DateTime.Today;
        public bool HasContractor { get; set; }
        public bool HasRequest { get; set; }
        public bool HasService { get; set; }
        public List<OfferContractorDto> Contracts { get; set; } = [];
        public List<OfferContractorDto> Requests { get; set; } = [];
        public List<OfferContractorDto> Services { get; set; } = [];
    }
}