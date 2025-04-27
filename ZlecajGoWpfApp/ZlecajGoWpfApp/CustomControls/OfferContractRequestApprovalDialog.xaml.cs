using System.Windows;
using ZlecajGoApi;
using ZlecajGoApi.Dtos;
using ZlecajGoWpfApp.Enums;

namespace ZlecajGoWpfApp.CustomControls;

public partial class OfferContractRequestApprovalDialog : BaseDialog
{
    public OfferContractRequestApprovalDialog(IApiClient apiClient, OfferContractorDto offerContractorDto, string message)
    {
        InitializeComponent();
        
        _apiClient = apiClient;
        _offerContractorDto = offerContractorDto;
        
        Title = "Zgoda na wykonanie";
        Message.Text = message;
    }

    private readonly IApiClient _apiClient;
    private readonly OfferContractorDto _offerContractorDto;
    
    private void RejectButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    private async void AcceptButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await _apiClient.CreateOfferContractAsync(_offerContractorDto);
        }
        catch (Exception)
        {
            CustomMessageBox.Show
            (
                "Wystąpił błąd podczas akceptacji oferty. Proszę spróbować ponownie później.",
                CustomMessageBoxType.Error,
                "Błąd"
            );
        }
        finally
        {
            DialogResult = true;
            Close();
        }
    }
}