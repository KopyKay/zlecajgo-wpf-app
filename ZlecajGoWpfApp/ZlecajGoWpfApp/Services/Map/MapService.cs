using System.Windows.Input;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;

namespace ZlecajGoWpfApp.Services.Map;

public class MapService : IMapService
{
    public MapService()
    {
        InitializeMap();
    }

    public GMapControl MapControl { get; private set; } = null!;
    
    private void InitializeMap()
    {
        MapControl = new GMapControl();
        
        const double lat = 52.65208303015435;
        const double lng = 19.06536041557555;
        
        MapControl.CacheLocation = Environment.CurrentDirectory + @"\GMapCache\";
        GMapProvider.Language = LanguageType.Polish;
        MapControl.MapProvider = GMapProviders.OpenStreetMap;

        MapControl.MinZoom = 6;
        MapControl.MaxZoom = 20;
        MapControl.Zoom = 12.0;

        MapControl.ShowCenter = false;
        MapControl.Position = new PointLatLng(lat, lng);
        MapControl.BoundsOfMap = new RectLatLng(85, -180, 360, 170);
        
        MapControl.DragButton = MouseButton.Left;
        MapControl.MouseWheelZoomType = MouseWheelZoomType.MousePositionWithoutCenter;
    }
}