namespace ZlecajGoWpfApp.Models;

public class OfferContractDetails
{
    public string TypeName { get; set; } = null!;
    public int TypeId { get; set; }
    public string ProviderFullName { get; set; } = null!;
    public string ProviderPhoneNumber { get; set; } = null!;
    public string ExecutorFullName { get; set; } = null!;
    public string ExecutorPhoneNumber { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string ZipCode { get; set; } = null!;
    public string City { get; set; } = null!;
    public string Street { get; set; } = null!;
    public DateTime StartDateTime { get; set; }
}