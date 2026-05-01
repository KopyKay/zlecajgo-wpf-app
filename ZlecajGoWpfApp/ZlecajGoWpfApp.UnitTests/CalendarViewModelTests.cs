using System.Globalization;
using FluentAssertions;
using NSubstitute;
using ZlecajGoApi;
using ZlecajGoApi.Dtos;
using ZlecajGoWpfApp.Enums;
using ZlecajGoWpfApp.Services.Navigation;
using ZlecajGoWpfApp.Services.Snackbar;
using ZlecajGoWpfApp.UnitTests.Helpers;
using ZlecajGoWpfApp.ViewModels;

namespace ZlecajGoWpfApp.UnitTests;

public class CalendarViewModelTests
{
    [StaFact]
    public void Initialize_DefaultMonth_Generates42Days()
    {
        // Arrange
        var viewModel = CreateViewModel();

        // Act
        ConfigureMonthYear(viewModel, 2024, 1);

        // Assert
        viewModel.Days.Should().HaveCount(42);
    }

    [StaFact]
    public void UpdateCalendar_MonthStartingOnSunday_PlacesFirstDayAtEndOfFirstWeek()
    {
        // Arrange
        var viewModel = CreateViewModel();

        // Act
        ConfigureMonthYear(viewModel, 2021, 8);

        // Assert
        var firstRowLastCell = viewModel.Days[6];
        firstRowLastCell.Day.Should().Be(1);
        firstRowLastCell.IsCurrentMonth.Should().BeTrue();
    }

    [StaFact]
    public void UpdateCalendar_MonthStartingOnMonday_PlacesFirstDayAtStart()
    {
        // Arrange
        var viewModel = CreateViewModel();

        // Act
        ConfigureMonthYear(viewModel, 2021, 11);

        // Assert
        var firstCell = viewModel.Days[0];
        firstCell.Day.Should().Be(1);
        firstCell.IsCurrentMonth.Should().BeTrue();
    }

    [StaFact]
    public void UpdateCalendar_ContractsByOfferType_AreClassifiedCorrectly()
    {
        // Arrange
        var viewModel = CreateViewModel();
        var date = new DateTime(2024, 6, 10);

        var requestOfferId = Guid.NewGuid();
        var serviceOfferId = Guid.NewGuid();

        viewModel.Offers = new()
        {
            new OfferDto { Id = requestOfferId, TypeId = (int)OfferType.Request },
            new OfferDto { Id = serviceOfferId, TypeId = (int)OfferType.Service }
        };

        viewModel.ProvidedOffers.Add(TestDataFactory.CreateOfferContractor(requestOfferId, "c1", date));
        viewModel.ProvidedOffers.Add(TestDataFactory.CreateOfferContractor(serviceOfferId, "c2", date));
        viewModel.ContractedOffers.Add(TestDataFactory.CreateOfferContractor(requestOfferId, "c3", date));
        viewModel.ContractedOffers.Add(TestDataFactory.CreateOfferContractor(serviceOfferId, "c4", date));

        // Act
        ConfigureMonthYear(viewModel, 2024, 6);

        // Assert
        var day = viewModel.Days.First(d => d.Date.Date == date.Date);
        day.HasContractor.Should().BeTrue();
        day.HasRequest.Should().BeTrue();
        day.HasService.Should().BeTrue();
        day.Contracts.Should().HaveCount(2);
        day.Requests.Should().HaveCount(1);
        day.Services.Should().HaveCount(1);
    }

    [StaFact]
    public void ChangingSelectedMonth_RegeneratesDays()
    {
        // Arrange
        var viewModel = CreateViewModel();

        // Act
        ConfigureMonthYear(viewModel, 2024, 1);
        var firstDate = viewModel.Days[0].Date;

        ConfigureMonthYear(viewModel, 2024, 2);
        var secondDate = viewModel.Days[0].Date;

        // Assert
        secondDate.Should().NotBe(firstDate);
    }

    private static CalendarViewModel CreateViewModel()
    {
        var navigationService = Substitute.For<INavigationService>();
        var snackbarService = Substitute.For<ISnackbarService>();
        var apiClient = Substitute.For<IApiClient>();

        return new CalendarViewModel(navigationService, snackbarService, apiClient);
    }

    private static void ConfigureMonthYear(CalendarViewModel viewModel, int year, int month)
    {
        var culture = new CultureInfo("pl-PL");
        var months = Enumerable.Range(1, 12)
            .Select(m =>
            {
                var monthName = culture.DateTimeFormat.GetMonthName(m);
                return char.ToUpper(monthName[0]) + monthName[1..];
            })
            .ToList();

        viewModel.Months = months;
        viewModel.Years = Enumerable.Range(year - 1, 3).Select(y => y.ToString()).ToList();
        viewModel.SelectedYear = year.ToString();
        viewModel.SelectedMonth = months[month - 1];
    }
}