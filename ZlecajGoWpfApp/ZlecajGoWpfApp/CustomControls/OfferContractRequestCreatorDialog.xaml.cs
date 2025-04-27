using System.Windows;
using ZlecajGoApi;
using ZlecajGoApi.Dtos;
using ZlecajGoApi.Hubs.Chat;
using ZlecajGoApi.Hubs.Notification;
using ZlecajGoWpfApp.Enums;

namespace ZlecajGoWpfApp.CustomControls;

public partial class OfferContractRequestCreatorDialog : BaseDialog
{
    public OfferContractRequestCreatorDialog(IChatHubClient chatHubClient, INotificationHubClient notificationHubClient, 
        ChatDto chat, List<OfferDto> partnerOffers)
    {
        InitializeComponent();
        
        _chatHubClient = chatHubClient;
        _notificationHubClient = notificationHubClient;
        _chatId = chat.Id;
        
        OffersComboBox.ItemsSource = partnerOffers;
        PartnerFullNameTextBox.Text = chat.ChatPartnerFullName;

        foreach (var o in partnerOffers)
        {
            o.TypeName = o.TypeId == (int)OfferType.Request
                ? RequestTypeName
                : ServiceTypeName;
        }
    }

    private readonly IChatHubClient _chatHubClient;
    private readonly INotificationHubClient _notificationHubClient;
    private readonly Guid _chatId;
    
    private const string RequestTypeName = "Zlecenie";
    private const string ServiceTypeName = "Usługa";
    
    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    private void SendRequestButton_Click(object sender, RoutedEventArgs e)
    {
        if (OffersComboBox.SelectedItem is null ||
            string.IsNullOrEmpty(DatePicker.Text) ||
            string.IsNullOrEmpty(TimePicker.Text))
        {
            
            ValidationMessageTextBlock.Text = "Wypełnij wszystkie pola!";
            ValidationMessageTextBlock.Visibility = Visibility.Visible;
            return;
        }

        var selectedDate = DatePicker.SelectedDate!.Value;
        var selectedTime = TimePicker.SelectedTime!.Value;

        var startDateTime = selectedDate.Date.Add(selectedTime.TimeOfDay);

        if (startDateTime <= DateTime.Now)
        {
            ValidationMessageTextBlock.Text = "Data i czas muszą być w przyszłości!";
            ValidationMessageTextBlock.Visibility = Visibility.Visible;
            return;
        }
        
        ValidationMessageTextBlock.Visibility = Visibility.Collapsed;
        
        var currentUser = UserSession.Instance.CurrentUser;
        var selectedOffer = (OfferDto)OffersComboBox.SelectedItem;
        
        var offerContractorDto = new OfferContractorDto
        {
            OfferId = selectedOffer.Id,
            ContractorId = currentUser.Id,
            StartDateTime = startDateTime,
            StatusId = (int)OfferStatus.Scheduled
        };
        var offerProviderId = selectedOffer.ProviderId;
        
        string chatMessage, notificationMessage;
        if (selectedOffer.TypeId == (int)OfferType.Request)
        {
            chatMessage = 
                $"Wysłałem(-am) prośbę o wykonanie przeze mnie zlecenia:\n• {selectedOffer.Title}\n• Z dnia {selectedOffer.PostDateTime:dd/MM/yyyy}";
            
            notificationMessage = 
                $"Użytkownik {currentUser.FullName} ({currentUser.UserName}) chce zobowiązać się do wykonania:\n\n" +
                $"Zlecenia: {selectedOffer.Title}\n\nDnia: {startDateTime:dd/MM/yyyy}\n\nO godzinie: {startDateTime:HH:mm}";
        }
        else
        {
            chatMessage = 
                $"Wysłałem(-am) prośbę o wykonanie dla mnie usługi:\n• {selectedOffer.Title}\n• Z dnia {selectedOffer.PostDateTime:dd/MM/yyyy}";
            
            notificationMessage = 
                $"Użytkownik {currentUser.FullName} ({currentUser.UserName}) prosi o wykonanie:\n\n" +
                $"Usługi: {selectedOffer.Title}\n\nDnia: {startDateTime:dd/MM/yyyy}\n\nO godzinie: {startDateTime:HH:mm}";
        }

        _chatHubClient.SendMessageAsync(_chatId, chatMessage);
        _notificationHubClient.SendOfferContractNotificationAsync(offerContractorDto, offerProviderId, notificationMessage);
        
        DialogResult = true;
        Close();
    }
}