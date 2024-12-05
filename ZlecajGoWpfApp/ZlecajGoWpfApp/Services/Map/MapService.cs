using System.Windows.Input;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;

namespace ZlecajGoWpfApp.Services.Map;

public class MapService : IMapService
{
    public void InitializeMap(GMapControl map)
    {
        const double lat = 52.65208303015435;
        const double lng = 19.06536041557555;
        
        map.CacheLocation = Environment.CurrentDirectory + @"\GMapCache\";
        GMapProvider.Language = LanguageType.Polish;
        map.MapProvider = GMapProviders.OpenStreetMap;

        map.MinZoom = 6;
        map.MaxZoom = 20;
        map.Zoom = 12.0;

        map.ShowCenter = false;
        map.Position = new PointLatLng(lat, lng);
        map.BoundsOfMap = new RectLatLng(85, -180, 360, 170);
        
        map.DragButton = MouseButton.Left;
        map.MouseWheelZoomType = MouseWheelZoomType.MousePositionWithoutCenter;
    }

    public void AddMarkers(GMapControl map, ICollection<GMapMarker> markers)
    {
        if (map.Markers.Count != 0)
        {
            map.Markers.Clear();
        }

        foreach (var marker in markers)
        {
            map.Markers.Add(marker);
        }
    }
}