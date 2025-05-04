using System.Windows;
using ZlecajGoWpfApp.Models;

namespace ZlecajGoWpfApp.CustomControls;

public partial class OfferContractDetailsDialog : BaseDialog
{
    public OfferContractDetailsDialog(List<OfferContractDetails> details)
    {
        InitializeComponent();

        var data = new
        {
            details[0].TypeName,
            details[0].TypeId,
            details.Count,
            Details = details
        };
        
        DataContext = data;
        ListView.ItemsSource = details;
    }
    
    private void CloseWindow(object sender, RoutedEventArgs routedEventArgs) => Close();
}