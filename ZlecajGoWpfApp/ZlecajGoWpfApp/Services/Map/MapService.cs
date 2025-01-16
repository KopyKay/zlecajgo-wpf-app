using System.Globalization;
using System.Net.Http;
using System.Windows.Input;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;
using Newtonsoft.Json.Linq;
using ZlecajGoApi.Exceptions;

namespace ZlecajGoWpfApp.Services.Map;

public class MapService : IMapService
{
    public MapService()
    {
        InitializeMap();
    }

    public GMapControl MapControl { get; private set; } = null!;

    private const string NominatimUrl = "https://nominatim.openstreetmap.org/";
    private const string NominatimSearchEndpoint = "search?format=jsonv2";
    
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

    public async Task<(double lat, double lon)> GetCoordinates(string postalCode, string place, string street)
    {
        var nominatimQuery = $"{NominatimUrl}{NominatimSearchEndpoint}" +
                             $"&country=Poland" +
                             $"&postalcode={Uri.EscapeDataString(postalCode)}" +
                             $"&city={Uri.EscapeDataString(place)}" +
                             $"&street={Uri.EscapeDataString(street)}" +
                             $"&addressdetails=1";
        
        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("User-Agent", "WpfApp");
        
        var response = await httpClient.GetAsync(nominatimQuery);
        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadAsStringAsync();
        var json = JArray.Parse(jsonResponse);

        if (json.Count == 0)
        {
            throw new InvalidAddressException();
        }
        
        var firstResult = json[0];
        var houseNumber = firstResult["address"]?["house_number"]?.ToString();
        
        if (houseNumber is null)
        {
            throw new InvalidAddressException();
        }
        
        var lat = firstResult["lat"]?.ToString();
        var lon = firstResult["lon"]?.ToString();

        if (lat is null || lon is null)
        {
            throw new CoordinatesNotFoundException();
        }
        
        var latDouble = double.Parse(lat, CultureInfo.InvariantCulture);
        var lonDouble = double.Parse(lon, CultureInfo.InvariantCulture);
        
        return (latDouble, lonDouble);
    }
}