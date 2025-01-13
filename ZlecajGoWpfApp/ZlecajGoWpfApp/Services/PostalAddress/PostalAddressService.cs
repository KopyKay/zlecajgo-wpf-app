using System.IO;
using OfficeOpenXml;

namespace ZlecajGoWpfApp.Services.PostalAddress;

public class PostalAddressService
{
    public PostalAddressService()
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
    }
    
    private const string XlsxFileName = "pocztowe-numery-adresowe.xlsx";
    private const string XlsxFilePath = @$"data\{XlsxFileName}";
    private static readonly string ProjectDirectory = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\..\..\"));
    private static readonly string ProjectXlsxFilePath = Path.Combine(ProjectDirectory, XlsxFilePath);

    private Dictionary<string, List<string>>? _postalAddresses;
    
    public async Task<Dictionary<string, List<string>>?> GetPostalAddressesAsync()
    {
        if (_postalAddresses is null)
        {
            await Task.Run(LoadPostalAddressesFromFileAsync);
        }

        return _postalAddresses;
    }

    private async Task LoadPostalAddressesFromFileAsync()
    {
        try
        {
            if (!File.Exists(ProjectXlsxFilePath))
            {
                throw new FileNotFoundException($"Plik {XlsxFileName} nie został znaleziony!");
            }

            var xlsxFileInfo = new FileInfo(ProjectXlsxFilePath);
            using var package = new ExcelPackage(xlsxFileInfo);
            await package.LoadAsync(xlsxFileInfo);
        
            var worksheet = package.Workbook.Worksheets[0];
            var row = 2; // first row is a header

            _postalAddresses = new Dictionary<string, List<string>>();
            
            while (worksheet.Cells[row, 1].Text != string.Empty)
            {
                var postalCode = worksheet.Cells[row, 1].Text.Trim();
                var place = worksheet.Cells[row, 2].Text.Trim();
            
                if (!_postalAddresses.TryGetValue(postalCode, out var value))
                {
                    value = [];
                    _postalAddresses[postalCode] = value;
                }

                value.Add(place);

                row++;
            }
        }
        catch (FileNotFoundException)
        {
            throw;
        }
        catch (Exception)
        {
            _postalAddresses = null;
            throw new Exception("Wystąpił błąd podczas wczytywania danych adresowych!");
        }
    }
}