using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ZlecajGoApi.Dtos;

public class MessageDto : INotifyPropertyChanged
{
    public Guid Id { get; set; }
    public Guid ChatId { get; set; }
    public string SenderId { get; set; } = null!;
    public string MessageText { get; set; } = null!;
    public DateTime SentAt { get; set; }
    private bool _isRead;
    public bool IsRead
    {
        get => _isRead;
        set
        {
            if (_isRead == value) return;
            _isRead = value;
            OnPropertyChanged();
        }
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string propertyName = null!)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}