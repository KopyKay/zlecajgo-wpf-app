using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ZlecajGoWpfApp.ViewModels;

namespace ZlecajGoWpfApp.Views;

public partial class ChatWindow : Window
{
    public ChatWindow(ChatViewModel viewModel)
    {
        InitializeComponent();
        
        DataContext = viewModel;
        
        ChatViewModel.ChatWindow = this;
        
        Loaded += async (s, e) =>
        {
            await viewModel.InitializeChatAsync();
            viewModel.Messages.CollectionChanged += Message_CollectionChanged;
            
            if (viewModel.Messages.Count > 0) ScrollMessagesToBottom();
        };
        
        Closed += (s, e) =>
        {
            viewModel.Messages.CollectionChanged -= Message_CollectionChanged;
            viewModel.Dispose();
        };
    }

    private void Message_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
        {
            ScrollMessagesToBottom();
        }
    }
    
    private void MessageTextBox_OnKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter) return;
        
        if (Keyboard.Modifiers == ModifierKeys.Shift)
        {
            var caretIndex = MessageTextBox.CaretIndex;
            MessageTextBox.Text = MessageTextBox.Text.Insert(caretIndex, Environment.NewLine);
            MessageTextBox.CaretIndex = caretIndex + Environment.NewLine.Length;
        }
        else
        {
            if (DataContext is ChatViewModel viewModel && viewModel.SendMessageCommand.CanExecute(null))
            {
                viewModel.SendMessageCommand.Execute(null);
            }
        }
        
        e.Handled = true;
    }

    private void Message_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (sender is not FrameworkElement { Parent: StackPanel stackPanel }) return;
        
        var dateTextBlock = stackPanel.Children
            .OfType<TextBlock>()
            .FirstOrDefault(tb => tb.Name == "MessageDateTextBlock");

        if (dateTextBlock != null)
        {
            dateTextBlock.Visibility = dateTextBlock.Visibility == Visibility.Visible 
                ? Visibility.Collapsed
                : Visibility.Visible;
        }
    }
    
    private void ScrollMessagesToBottom()
    {
        if (MessagesScrollViewer is null) return;
        
        Dispatcher.InvokeAsync(() =>
        {
            MessagesScrollViewer.ScrollToEnd();
        }, System.Windows.Threading.DispatcherPriority.Loaded);
    }
}