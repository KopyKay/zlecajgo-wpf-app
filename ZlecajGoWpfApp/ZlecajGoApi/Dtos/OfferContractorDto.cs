namespace ZlecajGoApi.Dtos;

public class OfferContractorDto
{
    public Guid OfferId { get; set; }
    public string ContractorId { get; set; } = null!;
    public DateTime StartDateTime { get; set; }
    public int StatusId { get; set; }
}