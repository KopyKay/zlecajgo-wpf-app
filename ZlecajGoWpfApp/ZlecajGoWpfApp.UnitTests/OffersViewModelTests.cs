using System.Reflection;
using FluentAssertions;
using GMap.NET.WindowsPresentation;
using NSubstitute;
using ZlecajGoApi;
using ZlecajGoApi.Dtos;
using ZlecajGoApi.Hubs.Chat;
using ZlecajGoApi.Hubs.Notification;
using ZlecajGoWpfApp.Enums;
using ZlecajGoWpfApp.Services.Map;
using ZlecajGoWpfApp.Services.Navigation;
using ZlecajGoWpfApp.Services.Snackbar;
using ZlecajGoWpfApp.UnitTests.Helpers;
using ZlecajGoWpfApp.ViewModels;
using System.Collections.ObjectModel;

namespace ZlecajGoWpfApp.UnitTests;

public class OffersViewModelTests
{
    [StaFact]
    public async Task FilterOffers_TypeCategoryCity_FiltersCorrectly()
    {
        // Arrange
        using var appScope = new TestApplicationScope();
        using var session = new UserSessionScope(TestDataFactory.CreateUser("user-1", "User One", "user1"));

        var viewModel = CreateViewModel();

        var available = new[]
        {
            TestDataFactory.CreateOffer(Guid.NewGuid(), "other", (int)OfferStatus.Pending, (int)OfferType.Request,
                "CategoryA", "Request", "Krakow", DateTime.Now.AddDays(1)),
            TestDataFactory.CreateOffer(Guid.NewGuid(), "other", (int)OfferStatus.Pending, (int)OfferType.Service,
                "CategoryA", "Service", "Krakow", DateTime.Now.AddDays(1)),
            TestDataFactory.CreateOffer(Guid.NewGuid(), "other", (int)OfferStatus.Pending, (int)OfferType.Request,
                "CategoryB", "Request", "Warszawa", DateTime.Now.AddDays(1))
        };

        viewModel.Offers = new ObservableCollection<OfferDto>(available);
        InvokeGetAvailableOffers(viewModel);

        viewModel.SelectedType = new TypeDto { Name = "Request" };
        viewModel.SelectedCategory = new CategoryDto { Name = "CategoryA" };
        viewModel.SelectedCity = "Krakow";

        // Act
        await viewModel.FilterOffersCommand.ExecuteAsync(null);

        // Assert
        var filtered = viewModel.AvailableOffersView!.Cast<OfferDto>().ToList();
        filtered.Should().ContainSingle(o => o.CategoryName == "CategoryA" && o.TypeName == "Request" && o.City == "Krakow");
    }

    [StaFact]
    public void AvailableOffers_InvalidItems_AreExcluded()
    {
        // Arrange
        using var appScope = new TestApplicationScope();
        using var session = new UserSessionScope(TestDataFactory.CreateUser("user-1", "User One", "user1"));

        var viewModel = CreateViewModel();

        var offers = new[]
        {
            TestDataFactory.CreateOffer(Guid.NewGuid(), "user-1", (int)OfferStatus.Pending, (int)OfferType.Request,
                "CategoryA", "Request", "City", DateTime.Now.AddDays(1)),
            TestDataFactory.CreateOffer(Guid.NewGuid(), "other", (int)OfferStatus.Taken, (int)OfferType.Request,
                "CategoryA", "Request", "City", DateTime.Now.AddDays(1)),
            TestDataFactory.CreateOffer(Guid.NewGuid(), "other", (int)OfferStatus.Pending, (int)OfferType.Request,
                "CategoryA", "Request", "City", DateTime.Now.AddDays(-1)),
            TestDataFactory.CreateOffer(Guid.NewGuid(), "other", (int)OfferStatus.Pending, (int)OfferType.Request,
                "CategoryA", "Request", "City", DateTime.Now.AddDays(1))
        };

        viewModel.Offers = new ObservableCollection<OfferDto>(offers);
        InvokeGetAvailableOffers(viewModel);

        // Act
        var available = viewModel.AvailableOffersView!.Cast<OfferDto>().ToList();

        // Assert
        available.Should().ContainSingle(o => o.ProviderId == "other" && o.ExpiryDateTime > DateTime.Now);
    }

    [StaFact]
    public async Task ResetFilters_AfterFiltering_RestoresAllOffers()
    {
        // Arrange
        using var appScope = new TestApplicationScope();
        using var session = new UserSessionScope(TestDataFactory.CreateUser("user-1", "User One", "user1"));

        var viewModel = CreateViewModel();

        var offers = new[]
        {
            TestDataFactory.CreateOffer(Guid.NewGuid(), "other", (int)OfferStatus.Pending, (int)OfferType.Request,
                "CategoryA", "Request", "Krakow", DateTime.Now.AddDays(1)),
            TestDataFactory.CreateOffer(Guid.NewGuid(), "other", (int)OfferStatus.Pending, (int)OfferType.Service,
                "CategoryB", "Service", "Warszawa", DateTime.Now.AddDays(1))
        };

        viewModel.Offers = new ObservableCollection<OfferDto>(offers);
        InvokeGetAvailableOffers(viewModel);

        viewModel.SelectedType = new TypeDto { Name = "Request" };
        viewModel.SelectedCategory = new CategoryDto { Name = "CategoryA" };
        viewModel.SelectedCity = "Krakow";

        // Act
        await viewModel.FilterOffersCommand.ExecuteAsync(null);
        await viewModel.ResetFilterOptionsCommand.ExecuteAsync(null);

        // Assert
        viewModel.SelectedType.Should().BeNull();
        viewModel.SelectedCategory.Should().BeNull();
        viewModel.SelectedCity.Should().BeNull();
        viewModel.AvailableOffersView!.Filter.Should().BeNull();

        var available = viewModel.AvailableOffersView.Cast<OfferDto>().ToList();
        available.Should().HaveCount(2);
    }

    private static OffersViewModel CreateViewModel()
    {
        var navigationService = Substitute.For<INavigationService>();
        var snackbarService = Substitute.For<ISnackbarService>();
        var apiClient = Substitute.For<IApiClient>();
        var mapService = Substitute.For<IMapService>();
        var chatHubClient = Substitute.For<IChatHubClient>();
        var notificationHubClient = Substitute.For<INotificationHubClient>();

        mapService.MapControl.Returns(new GMapControl());

        return new OffersViewModel(navigationService, snackbarService, apiClient,
            Substitute.For<IServiceProvider>(), mapService, chatHubClient, notificationHubClient);
    }

    private static void InvokeGetAvailableOffers(OffersViewModel viewModel)
    {
        var method = typeof(OffersViewModel).GetMethod("GetAvailableOffers", BindingFlags.Instance | BindingFlags.NonPublic);
        method.Should().NotBeNull();
        method.Invoke(viewModel, null);
    }
}