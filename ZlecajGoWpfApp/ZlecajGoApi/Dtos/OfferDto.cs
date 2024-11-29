using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ZlecajGoApi.Dtos;

public class OfferDto : INotifyPropertyChanged
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public decimal Price { get; set; }
    public DateTime PostDateTime { get; set; }
    public DateTime ExpiryDateTime { get; set; }
    public string[]? ImageUrls { get; set; }
    public string City { get; set; } = null!;
    public string Street { get; set; } = null!;
    public string ZipCode { get; set; } = null!;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public int CategoryId { get; set; }
    public int StatusId { get; set; }
    public int TypeId { get; set; }
    public string ProviderId { get; set; } = null!;

    private string _categoryName = string.Empty;
    public string CategoryName
    {
        get => _categoryName;
        set
        {
            if (_categoryName == value) return;
            _categoryName = value;
            OnPropertyChanged();
        }
    }

    private string _statusName = string.Empty;
    public string StatusName
    {
        get => _statusName;
        set
        {
            if (_statusName == value) return;
            _statusName = value;
            OnPropertyChanged();
        }
    }

    private string _typeName = string.Empty;
    public string TypeName
    {
        get => _typeName;
        set
        {
            if (_typeName == value) return;
            _typeName = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string propertyName = null!)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}