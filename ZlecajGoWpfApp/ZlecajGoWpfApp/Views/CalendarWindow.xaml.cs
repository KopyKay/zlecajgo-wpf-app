using System.Windows;
using ZlecajGoApi;
using ZlecajGoApi.Dtos;
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
}