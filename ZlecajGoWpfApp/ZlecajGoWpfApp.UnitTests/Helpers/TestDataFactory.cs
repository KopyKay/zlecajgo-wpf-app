using ZlecajGoApi.Dtos;
using ZlecajGoWpfApp.Enums;

namespace ZlecajGoWpfApp.UnitTests.Helpers;

public static class TestDataFactory
{
    public static UserDto CreateUser(string id, string fullName, string userName, string email = "user@example.com") => new()
    {
        Id = id,
        FullName = fullName,
        UserName = userName,
        Email = email,
        AccessToken = "token",
        RefreshToken = "refresh",
        Password = "Secret1!"
    };

    public static OfferDto CreateOffer(Guid id, string providerId, int statusId, int typeId, string categoryName,
        string typeName, string city, DateTime expiry) => new()
    {
        Id = id,
        Title = "Offer",
        Description = "Description",
        Price = 10m,
        PostDateTime = DateTime.Now.AddDays(-1),
        ExpiryDateTime = expiry,
        City = city,
        Street = "Main 1",
        ZipCode = "00-001",
        Latitude = 10.0,
        Longitude = 20.0,
        CategoryId = 1,
        StatusId = statusId,
        TypeId = typeId,
        ProviderId = providerId,
        CategoryName = categoryName,
        StatusName = statusId == (int)OfferStatus.Pending ? "Pending" : "Other",
        TypeName = typeName,
        ProviderFullName = "Provider"
    };

    public static ChatDto CreateChat(Guid id, string user1Id, string user2Id, string user1FullName, string user2FullName) => new()
    {
        Id = id,
        User1Id = user1Id,
        User2Id = user2Id,
        CreatedAt = DateTime.Now.AddDays(-2),
        User1FullName = user1FullName,
        User2FullName = user2FullName
    };

    public static OfferContractorDto CreateOfferContractor(Guid offerId, string contractorId, DateTime startDate) => new()
    {
        OfferId = offerId,
        ContractorId = contractorId,
        StartDateTime = startDate,
        StatusId = 1
    };
}