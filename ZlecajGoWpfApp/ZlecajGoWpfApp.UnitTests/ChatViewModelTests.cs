using FluentAssertions;
using NSubstitute;
using ZlecajGoApi;
using ZlecajGoApi.Hubs.Chat;
using ZlecajGoApi.Hubs.Notification;
using ZlecajGoWpfApp.Services.Navigation;
using ZlecajGoWpfApp.Services.Snackbar;
using ZlecajGoWpfApp.UnitTests.Helpers;
using ZlecajGoWpfApp.ViewModels;
using ZlecajGoApi.Dtos;

namespace ZlecajGoWpfApp.UnitTests;

public class ChatViewModelTests
{
    [StaFact]
    public void SearchText_PartnerName_FiltersConversations()
    {
        // Arrange
        using var session = new UserSessionScope(TestDataFactory.CreateUser("user-1", "User One", "user1"));

        var viewModel = CreateViewModel();

        viewModel.Chats.Add(TestDataFactory.CreateChat(Guid.NewGuid(), "user-1", "user-2", "User One", "Anna Kowalska"));
        viewModel.Chats.Add(TestDataFactory.CreateChat(Guid.NewGuid(), "user-1", "user-3", "User One", "Piotr Nowak"));

        // Act
        viewModel.SearchText = "anna";

        // Assert
        var filtered = viewModel.FilteredChats!.Cast<ChatDto>().ToList();
        filtered.Should().ContainSingle();
        filtered[0].User2FullName.Should().Be("Anna Kowalska");
    }

    [StaFact]
    public void SearchText_Empty_ReturnsAllConversations()
    {
        // Arrange
        using var session = new UserSessionScope(TestDataFactory.CreateUser("user-1", "User One", "user1"));

        var viewModel = CreateViewModel();

        viewModel.Chats.Add(TestDataFactory.CreateChat(Guid.NewGuid(), "user-1", "user-2", "User One", "Anna Kowalska"));
        viewModel.Chats.Add(TestDataFactory.CreateChat(Guid.NewGuid(), "user-1", "user-3", "User One", "Piotr Nowak"));

        // Act
        viewModel.SearchText = string.Empty;

        // Assert
        var filtered = viewModel.FilteredChats!.Cast<ChatDto>().ToList();
        filtered.Should().HaveCount(2);
    }

    private static ChatViewModel CreateViewModel()
    {
        var navigationService = Substitute.For<INavigationService>();
        var snackbarService = Substitute.For<ISnackbarService>();
        var apiClient = Substitute.For<IApiClient>();
        var chatHubClient = Substitute.For<IChatHubClient>();
        var notificationHubClient = Substitute.For<INotificationHubClient>();

        return new ChatViewModel(navigationService, snackbarService, apiClient, chatHubClient, notificationHubClient);
    }
}