using FluentAssertions;
using NSubstitute;
using ZlecajGoApi;
using ZlecajGoApi.Dtos;
using ZlecajGoWpfApp.Services.Navigation;
using ZlecajGoWpfApp.Services.Snackbar;
using ZlecajGoWpfApp.ViewModels;
using ZlecajGoWpfApp.Views;
using System.Windows;
using System.Windows.Controls;

namespace ZlecajGoWpfApp.UnitTests;

public class LogInViewModelTests
{
    [Fact]
    public async Task LogIn_SuccessWithCompletedProfile_NavigatesToOffers()
    {
        // Arrange
        var navigationService = Substitute.For<INavigationService>();
        var snackbarService = Substitute.For<ISnackbarService>();
        var apiClient = Substitute.For<IApiClient>();
        apiClient.LoginAsync(Arg.Any<LogInDto>()).Returns(true);

        var viewModel = new LogInViewModel(navigationService, snackbarService, apiClient)
        {
            Email = "test@example.com",
            Password = "Password1!"
        };

        // Act
        await viewModel.LogInCommand.ExecuteAsync(null);

        // Assert
        navigationService.Received(1)
            .NavigateTo<OffersPage>(Arg.Any<Window?>(), Arg.Any<Frame?>());
        await apiClient.Received(1).LoginAsync(Arg.Any<LogInDto>());
    }

    [Fact]
    public async Task LogIn_SuccessWithIncompleteProfile_NavigatesToSetup()
    {
        // Arrange
        var navigationService = Substitute.For<INavigationService>();
        var snackbarService = Substitute.For<ISnackbarService>();
        var apiClient = Substitute.For<IApiClient>();
        apiClient.LoginAsync(Arg.Any<LogInDto>()).Returns(false);

        var viewModel = new LogInViewModel(navigationService, snackbarService, apiClient)
        {
            Email = "test@example.com",
            Password = "Password1!"
        };

        // Act
        await viewModel.LogInCommand.ExecuteAsync(null);

        // Assert
        navigationService.Received(1)
            .NavigateTo<SetUpUserCredentialsPage>(Arg.Any<Window?>(), Arg.Any<Frame?>());
        snackbarService.Received(1).EnqueueMessage("Należy uzupełnić dane użytkownika!");
    }

    [Fact]
    public async Task LogIn_Unauthorized_ShowsErrorMessage()
    {
        // Arrange
        var navigationService = Substitute.For<INavigationService>();
        var snackbarService = Substitute.For<ISnackbarService>();
        var apiClient = Substitute.For<IApiClient>();
        apiClient.LoginAsync(Arg.Any<LogInDto>())
            .Returns(Task.FromException<bool>(new ZlecajGoApi.Exceptions.UnauthorizedAccessException()));

        var viewModel = new LogInViewModel(navigationService, snackbarService, apiClient)
        {
            Email = "test@example.com",
            Password = "Password1!"
        };

        // Act
        await viewModel.LogInCommand.ExecuteAsync(null);

        // Assert
        snackbarService.Received(1).EnqueueMessage("Nieautoryzowany użytkownik! Sprawdź dane logowania.");
        navigationService.DidNotReceiveWithAnyArgs()
            .NavigateTo<OffersPage>();
    }

    [Fact]
    public async Task LogIn_EmptyEmail_BlocksApiCall()
    {
        // Arrange
        var navigationService = Substitute.For<INavigationService>();
        var snackbarService = Substitute.For<ISnackbarService>();
        var apiClient = Substitute.For<IApiClient>();

        var viewModel = new LogInViewModel(navigationService, snackbarService, apiClient)
        {
            Email = string.Empty,
            Password = "Password1!"
        };

        // Act
        await viewModel.LogInCommand.ExecuteAsync(null);

        // Assert
        await apiClient.DidNotReceive().LoginAsync(Arg.Any<LogInDto>());
        viewModel.HasErrors.Should().BeTrue();
    }

    [Fact]
    public async Task LogIn_EmptyPassword_BlocksApiCall()
    {
        // Arrange
        var navigationService = Substitute.For<INavigationService>();
        var snackbarService = Substitute.For<ISnackbarService>();
        var apiClient = Substitute.For<IApiClient>();

        var viewModel = new LogInViewModel(navigationService, snackbarService, apiClient)
        {
            Email = "test@example.com",
            Password = string.Empty
        };

        // Act
        await viewModel.LogInCommand.ExecuteAsync(null);

        // Assert
        await apiClient.DidNotReceive().LoginAsync(Arg.Any<LogInDto>());
        viewModel.HasErrors.Should().BeTrue();
    }
}