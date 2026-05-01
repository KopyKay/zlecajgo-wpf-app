using FluentAssertions;
using NSubstitute;
using ZlecajGoApi;
using ZlecajGoApi.Dtos;
using ZlecajGoApi.Exceptions;
using ZlecajGoWpfApp.Services.Map;
using ZlecajGoWpfApp.Services.Navigation;
using ZlecajGoWpfApp.Services.PostalAddress;
using ZlecajGoWpfApp.Services.Snackbar;
using ZlecajGoWpfApp.UnitTests.Helpers;
using ZlecajGoWpfApp.ViewModels;

namespace ZlecajGoWpfApp.UnitTests;

public class CreateOfferViewModelTests
{
    [StaFact]
    public async Task PostalCode_ValidCode_FiltersPlacesView()
    {
        // Arrange
        using var appScope = new TestApplicationScope();

        var postalAddressService = Substitute.For<IPostalAddressService>();
        var mapService = Substitute.For<IMapService>();
        var (viewModel, _) = CreateViewModel(postalAddressService, mapService);

        var places = new Dictionary<string, List<string>>
        {
            ["00-001"] = ["CityA", "CityB"],
            ["00-002"] = ["CityC"]
        };

        postalAddressService.TryGetPostalAddressesAsync().Returns(places);

        // Act
        await viewModel.LoadPostalAddressesAsync();
        viewModel.PostalCode = "00-001";

        // Assert
        var filtered = viewModel.PlacesView!.Cast<string>().ToList();
        filtered.Should().BeEquivalentTo("CityA", "CityB");
    }

    [StaFact]
    public async Task PostalCode_SingleMatchingPlace_AutoSelectsPlace()
    {
        // Arrange
        using var appScope = new TestApplicationScope();

        var postalAddressService = Substitute.For<IPostalAddressService>();
        var mapService = Substitute.For<IMapService>();
        var (viewModel, _) = CreateViewModel(postalAddressService, mapService);

        var places = new Dictionary<string, List<string>>
        {
            ["00-003"] = ["OnlyCity"]
        };

        postalAddressService.TryGetPostalAddressesAsync().Returns(places);

        // Act
        await viewModel.LoadPostalAddressesAsync();
        viewModel.PostalCode = "00-003";

        // Assert
        viewModel.SelectedPlace.Should().Be("OnlyCity");
    }

    [StaFact]
    public async Task AddOffer_InvalidAddress_DoesNotCreateOffer()
    {
        // Arrange
        using var appScope = new TestApplicationScope();
        using var messageScope = new CustomMessageBoxScope();

        var postalAddressService = Substitute.For<IPostalAddressService>();
        var mapService = Substitute.For<IMapService>();
        var (viewModel, apiClient) = CreateViewModel(postalAddressService, mapService);

        mapService.TryGetCoordinates(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(Task.FromException<(double lat, double lon)>(new InvalidAddressException()));

        SetValidOfferFields(viewModel);

        // Act
        await viewModel.AddOfferCommand.ExecuteAsync(null);

        // Assert
        await apiClient.DidNotReceive().CreateOfferAsync(Arg.Any<OfferDto>());
    }

    [StaFact]
    public async Task AddOffer_CoordinatesNotFound_DoesNotCreateOffer()
    {
        // Arrange
        using var appScope = new TestApplicationScope();
        using var messageScope = new CustomMessageBoxScope();

        var postalAddressService = Substitute.For<IPostalAddressService>();
        var mapService = Substitute.For<IMapService>();
        var (viewModel, apiClient) = CreateViewModel(postalAddressService, mapService);

        mapService.TryGetCoordinates(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(Task.FromException<(double lat, double lon)>(new CoordinatesNotFoundException()));

        SetValidOfferFields(viewModel);

        // Act
        await viewModel.AddOfferCommand.ExecuteAsync(null);

        // Assert
        await apiClient.DidNotReceive().CreateOfferAsync(Arg.Any<OfferDto>());
    }

    private static (CreateOfferViewModel viewModel, IApiClient apiClient) CreateViewModel(
        IPostalAddressService postalAddressService,
        IMapService mapService)
    {
        var navigationService = Substitute.For<INavigationService>();
        var snackbarService = Substitute.For<ISnackbarService>();
        var apiClient = Substitute.For<IApiClient>();

        var viewModel = new CreateOfferViewModel(navigationService, snackbarService, apiClient, postalAddressService, mapService)
        {
            OfferTypes = [new TypeDto { Id = 1, Name = "Request" }],
            OfferCategories = [new CategoryDto { Id = 1, Name = "Category" }]
        };

        return (viewModel, apiClient);
    }

    private static void SetValidOfferFields(CreateOfferViewModel viewModel)
    {
        viewModel.SelectedOfferType = viewModel.OfferTypes.First();
        viewModel.SelectedOfferCategory = viewModel.OfferCategories.First();
        viewModel.OfferTitle = "Title";
        viewModel.OfferDescription = "Description";
        viewModel.PostalCode = "00-001";
        viewModel.SelectedPlace = "City";
        viewModel.StreetNumber = "1";
        viewModel.SelectedDurationInDays = 1;
        viewModel.OfferPrice = "10";
    }
}