using GMap.NET.WindowsPresentation;

namespace ZlecajGoWpfApp.Services.Map;

public interface IMapService
{
    void InitializeMap(GMapControl map);
    void AddMarkers(GMapControl map, ICollection<GMapMarker> markers);
}