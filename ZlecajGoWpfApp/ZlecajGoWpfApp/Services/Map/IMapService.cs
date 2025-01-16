using GMap.NET.WindowsPresentation;
using ZlecajGoApi.Dtos;

namespace ZlecajGoWpfApp.Services.Map;

public interface IMapService
{
    GMapControl MapControl { get; }
    Task<(double lat, double lon)> GetCoordinates(string postalCode, string place, string street);
}