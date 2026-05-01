using System.Collections.Generic;
using System.Threading.Tasks;

namespace ZlecajGoWpfApp.Services.PostalAddress;

public interface IPostalAddressService
{
    Task<Dictionary<string, List<string>>?> TryGetPostalAddressesAsync();
}