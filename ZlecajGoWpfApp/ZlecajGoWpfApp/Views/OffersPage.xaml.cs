using System.Windows.Controls;
using ZlecajGoWpfApp.ViewModels;

namespace ZlecajGoWpfApp.Views;

public partial class OffersPage : Page
{
    public OffersPage(OffersViewModel viewModel)
    {
        InitializeComponent();

        DataContext = viewModel;

        Loaded += async (s, e) => await viewModel.InitializeOffersAndMapAsync();
    }
}