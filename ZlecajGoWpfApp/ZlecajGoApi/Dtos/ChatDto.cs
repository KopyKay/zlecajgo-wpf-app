using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ZlecajGoApi.Dtos;

public class ChatDto : INotifyPropertyChanged
{
    public Guid Id { get; set; }
    public string User1Id { get; set; } = null!;
    public string User2Id { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime? LastMessageAt { get; set; }
    public List<MessageDto> Messages { get; set; } = [];
    
    private string _user1FullName = string.Empty;
    public string User1FullName
    {
        get => _user1FullName;
        set
        {
            if (_user1FullName == value) return;
            _user1FullName = value;
            OnPropertyChanged();
        }
    }
    
    private string _user2FullName = string.Empty;
    public string User2FullName
    {
        get => _user2FullName;
        set
        {
            if (_user2FullName == value) return;
            _user2FullName = value;
            OnPropertyChanged();
        }
    }
    
    private string _chatPartnerId = string.Empty;
    public string ChatPartnerId
    {
        get => _chatPartnerId;
        set
        {
            if (_chatPartnerId == value) return;
            _chatPartnerId = value;
            OnPropertyChanged();
        }
    }
    
    private string _chatPartnerFullName = string.Empty;
    public string ChatPartnerFullName
    {
        get => _chatPartnerFullName;
        set
        {
            if (_chatPartnerFullName == value) return;
            _chatPartnerFullName = value;
            OnPropertyChanged();
        }
    }
    
    private string _lastMessageText = string.Empty;
    public string LastMessageText
    {
        get => _lastMessageText;
        set
        {
            if (_lastMessageText == value) return;
            _lastMessageText = value;
            OnPropertyChanged();
        }
    }
    
    private bool _hasUnreadMessages;
    public bool HasUnreadMessages
    {
        get => _hasUnreadMessages;
        set
        {
            if (_hasUnreadMessages == value) return;
            _hasUnreadMessages = value;
            OnPropertyChanged();
        }
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string propertyName = null!)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}