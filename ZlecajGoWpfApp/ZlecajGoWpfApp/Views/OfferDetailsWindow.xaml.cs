using System.Windows;
using ZlecajGoWpfApp.ViewModels;

namespace ZlecajGoWpfApp.Views;

public partial class OfferDetailsWindow : Window
{
    public OfferDetailsWindow(OfferDetailsViewModel viewModel)
    {
        InitializeComponent();

        DataContext = viewModel;
    }
}