using GMap.NET.WindowsPresentation;
using ZlecajGoApi.Dtos;

namespace ZlecajGoWpfApp.Services.Map;

public interface IMapService
{
    GMapControl MapControl { get; }
}