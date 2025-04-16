using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ZlecajGoApi;
using ZlecajGoApi.Dtos;
using ZlecajGoApi.Exceptions;
using ZlecajGoApi.Hubs.Chat;
using ZlecajGoWpfApp.CustomControls;
using ZlecajGoWpfApp.Enums;
using ZlecajGoWpfApp.Services.Navigation;
using ZlecajGoWpfApp.Services.Snackbar;

namespace ZlecajGoWpfApp.ViewModels;

public partial class ChatViewModel : BaseViewModel, IDisposable
{
    public ChatViewModel(INavigationService navigationService, ISnackbarService snackbarService, IApiClient apiClient,
        IChatHubClient chatHubClient) : base(navigationService, snackbarService, apiClient)
    {
        Title = "Wiadomości";
        
        _chatHubClient = chatHubClient;
        _chatHubClient.OnChatReceived += HandleChatReceived;
        _chatHubClient.OnMessageReceived += HandleMessageReceived;
        _chatHubClient.OnMessagesReadReceived += HandleMessagesReadReceived;

        _currentUser = UserSession.Instance.CurrentUser;
        
        _filteredChats = CollectionViewSource.GetDefaultView(Chats);
        _filteredChats.Filter = FilterChats;
    }

    internal static Window? ChatWindow;
    
    private readonly IChatHubClient _chatHubClient;
    private readonly UserDto _currentUser;
    
    [ObservableProperty]
    private ObservableCollection<ChatDto> _chats = [];

    [ObservableProperty] 
    private ChatDto? _selectedChat;
    
    [ObservableProperty]
    private ObservableCollection<MessageDto> _messages = [];
    
    [ObservableProperty]
    private string _messageContent = string.Empty;

    [ObservableProperty]
    private string _searchText = string.Empty;
    
    [ObservableProperty]
    private ICollectionView? _filteredChats;

    [RelayCommand]
    private static void CloseWindow() => ChatWindow!.Close();
    
    [RelayCommand]
    private async Task SendMessageAsync()
    {
        if (SelectedChat == null || string.IsNullOrWhiteSpace(MessageContent))
            return;

        var message = MessageContent;
        MessageContent = string.Empty;
        
        await ExecuteChatHubOperationAsync(() => 
            _chatHubClient.SendMessageAsync(SelectedChat.Id, message));
    }
    
    public async Task InitializeChatAsync()
    {
        try
        {
            IsBusy = true;

            await FetchDataAsync(Chats, ApiClient.GetChatsAsync);

            if (Chats.Count == 0) return;
            
            SelectedChat = Chats.First(); 
            
            var users = await ApiClient.GetUsersAsync();

            if (users.Count == 0) return;
            
            foreach (var chat in Chats)
            {
                await SetChatDetailsAsync(chat, users);
            }
        }
        catch (Exception)
        {
            CloseWindow();
            SnackbarService.EnqueueMessage("Błąd podczas ładowania czatów! Spróbuj ponownie później.");
        }
        finally
        {
            IsBusy = false;
        }
    }
    
    private bool FilterChats(object obj)
    {
        if (string.IsNullOrWhiteSpace(SearchText)) return true;
        
        if (obj is not ChatDto chat) return false;

        var searchTerm = SearchText.ToLower();
        var partnerName = chat.User1Id == _currentUser.Id
            ? chat.User2FullName.ToLower()
            : chat.User1FullName.ToLower();
            
        return partnerName.Contains(searchTerm);
    }
    
    partial void OnSearchTextChanged(string value)
    {
        FilteredChats?.Refresh();
    }
    
    partial void OnSelectedChatChanged(ChatDto? oldValue, ChatDto? newValue)
    {
        MessageContent = string.Empty;
        
        if (newValue == null) return;

        newValue.HasUnreadMessages = false;
        
        Messages.Clear();

        if (newValue.Messages.Count == 0) return;

        foreach (var message in newValue.Messages.OrderBy(m => m.SentAt))
        {
            Messages.Add(message);
        }

        if (Messages.Any(m => !m.IsRead && m.SenderId != _currentUser.Id))
        {
            Task.Run(async () =>
                await ExecuteChatHubOperationAsync(() => _chatHubClient.ReadMessagesAsync(newValue.Id)));
        }
    }
    
    private async Task HandleChatReceived(ChatDto chatDto)
    {
        await Application.Current.Dispatcher.InvokeAsync(async () =>
        {
            var existingChat = Chats.FirstOrDefault(c => c.Id == chatDto.Id);

            if (existingChat != null)
            {
                var index = Chats.IndexOf(existingChat);
                Chats[index] = chatDto;
            }
            else
            {
                await SetChatDetailsAsync(chatDto);
                Chats.Add(chatDto);
            }

            SortChatsByLastMessageDate();
        });
    }
    
    private async Task HandleMessageReceived(MessageDto messageDto)
    {
        await Application.Current.Dispatcher.InvokeAsync(async () =>
        {
            var chat = Chats.FirstOrDefault(c => c.Id == messageDto.ChatId);
            if (chat == null) return;

            chat.Messages.Add(messageDto);
            chat.LastMessageAt = messageDto.SentAt;
            chat.LastMessageText = messageDto.SenderId == _currentUser.Id
                ? $"Ty: {messageDto.MessageText}"
                : messageDto.MessageText;

            if (messageDto.SenderId != _currentUser.Id && 
                (SelectedChat is null || SelectedChat.Id != chat.Id))
            {
                chat.HasUnreadMessages = true;
            }
            
            SortChatsByLastMessageDate();

            if (SelectedChat is not null && SelectedChat.Id == chat.Id)
            {
                Messages.Add(messageDto);

                await ExecuteChatHubOperationAsync(() =>
                    _chatHubClient.ReadMessagesAsync(messageDto.ChatId));
            }
        });
    }
    
    private async Task HandleMessagesReadReceived(List<Guid> messageIds)
    {
        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            if (SelectedChat is not null)
            {
                foreach (var message in SelectedChat.Messages.Where(m => 
                             messageIds.Contains(m.Id) && m.SenderId == _currentUser.Id))
                {
                    message.IsRead = true;
                }
            }
            
            foreach (var chat in Chats)
            {
                foreach (var message in chat.Messages.Where(m => 
                             messageIds.Contains(m.Id) && m.SenderId == _currentUser.Id))
                {
                    message.IsRead = true;
                }
            }
        });
    }

    private async Task SetChatDetailsAsync(ChatDto chat, List<UserDto>? users = null)
    {
        users ??= await ApiClient.GetUsersAsync();
        
        var currentUserIsUser1 = chat.User1Id == _currentUser.Id;
        var partnerId = currentUserIsUser1 ? chat.User2Id : chat.User1Id;
        var partnerUser = users!.First(u => u.Id == partnerId);
        var lastMessage = chat.Messages.OrderByDescending(m => m.SentAt).FirstOrDefault();
                
        if (currentUserIsUser1)
        {
            chat.User1FullName = _currentUser.FullName!;
            chat.User2FullName = partnerUser.FullName!;
        }
        else
        {
            chat.User1FullName = partnerUser.FullName!;
            chat.User2FullName = _currentUser.FullName!;
        }
                
        chat.ChatPartnerId = partnerId;
        chat.ChatPartnerFullName = partnerUser.FullName!;
                
        if (lastMessage is not null)
        {
            chat.LastMessageText = lastMessage.SenderId == _currentUser.Id 
                ? $"Ty: {lastMessage.MessageText}" 
                : lastMessage.MessageText;
        }
        else
        {
            chat.LastMessageText = string.Empty;
        }
                
        chat.HasUnreadMessages = chat.Messages.Any(m => !m.IsRead && m.SenderId != _currentUser.Id);
    }
    
    private static async Task ExecuteChatHubOperationAsync(Func<Task> operation, string? customErrorMessage = null)
    {
        try
        {
            await operation();
        }
        catch (ChatHubConnectionException e)
        {
            CustomMessageBox.Show(e.Message, CustomMessageBoxType.Error, "Błąd");
        }
        catch (Exception)
        {
            var message = customErrorMessage ?? "Wystąpił błąd! Nie można kontynuować akcji.";
            CustomMessageBox.Show(message, CustomMessageBoxType.Error, "Błąd");
        }
    }
    
    private void SortChatsByLastMessageDate()
    {
        var sortedChats = Chats.OrderByDescending(c => c.LastMessageAt ?? c.CreatedAt).ToList();
        
        for (var i = 0; i < sortedChats.Count; i++)
        {
            var currentIndex = Chats.IndexOf(sortedChats[i]);
        
            if (currentIndex != i)
            {
                Chats.Move(currentIndex, i);
            }
        }
    }
    
    public void Dispose()
    {
        _chatHubClient.OnChatReceived -= HandleChatReceived;
        _chatHubClient.OnMessageReceived -= HandleMessageReceived;
        _chatHubClient.OnMessagesReadReceived -= HandleMessagesReadReceived;
        
        GC.SuppressFinalize(this);
    }
}