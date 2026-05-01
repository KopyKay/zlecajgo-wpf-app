using FluentAssertions;
using NSubstitute;
using ZlecajGoApi;
using ZlecajGoApi.Exceptions;
using ZlecajGoWpfApp.Services.Navigation;
using ZlecajGoWpfApp.Services.Snackbar;
using ZlecajGoWpfApp.ViewModels;
using ZlecajGoApi.Dtos;

namespace ZlecajGoWpfApp.UnitTests;

public class SignUpViewModelTests
{
    [Theory]
    [InlineData("abcdef", true, false, false, false)]
    [InlineData("ABCDEF", false, true, false, false)]
    [InlineData("123456", false, false, true, false)]
    [InlineData("abc$%", true, false, false, true)]
    [InlineData("Ab1$xx", true, true, true, true)]
    public void Password_VariousInputs_SetsStrengthFlags(string password, bool lower, bool upper, bool number, bool special)
    {
        // Arrange
        var viewModel = CreateViewModel();

        // Act
        viewModel.Password = password;
        viewModel.ConfirmPassword = password;

        // Assert
        viewModel.HasLowerCase.Should().Be(lower);
        viewModel.HasUpperCase.Should().Be(upper);
        viewModel.HasNumber.Should().Be(number);
        viewModel.HasSpecialCharacter.Should().Be(special);
    }

    [Theory]
    [InlineData("Ab1$", false)]
    [InlineData("Ab1$xx", true)]
    public void Password_VariousLengths_ValidatesMinimumLength(string password, bool expected)
    {
        // Arrange
        var viewModel = CreateViewModel();

        // Act
        viewModel.Password = password;
        viewModel.ConfirmPassword = password;

        // Assert
        viewModel.HasMinimumLength.Should().Be(expected);
    }

    [Fact]
    public async Task SignUp_EmailAlreadyInUse_ShowsMessage()
    {
        // Arrange
        var navigationService = Substitute.For<INavigationService>();
        var snackbarService = Substitute.For<ISnackbarService>();
        var apiClient = Substitute.For<IApiClient>();
        apiClient.RegisterAsync(Arg.Any<SignUpDto>())
            .Returns(Task.FromException(new EmailAlreadyInUseException("Email zajety")));

        var viewModel = new SignUpViewModel(navigationService, snackbarService, apiClient)
        {
            Email = "test@example.com",
            Password = "Strong1$",
            ConfirmPassword = "Strong1$"
        };

        // Act
        await viewModel.SignUpCommand.ExecuteAsync(null);

        // Assert
        snackbarService.Received(1).EnqueueMessage("Adres email \"Email zajety\" jest już zarejestrowany!");
    }

    private static SignUpViewModel CreateViewModel()
    {
        var navigationService = Substitute.For<INavigationService>();
        var snackbarService = Substitute.For<ISnackbarService>();
        var apiClient = Substitute.For<IApiClient>();

        return new SignUpViewModel(navigationService, snackbarService, apiClient);
    }
}