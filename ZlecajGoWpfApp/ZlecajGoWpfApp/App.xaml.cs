using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ZlecajGoApi;
using ZlecajGoApi.Hubs.Chat;
using ZlecajGoWpfApp.Services.Map;
using ZlecajGoWpfApp.Services.Navigation;
using ZlecajGoWpfApp.Services.PostalAddress;
using ZlecajGoWpfApp.Services.Snackbar;
using ZlecajGoWpfApp.Views;
using ZlecajGoWpfApp.ViewModels;

namespace ZlecajGoWpfApp;

public partial class App : Application
{
    public App()
    {
        AppHost = Host.CreateDefaultBuilder()
            .ConfigureServices((hostContext, services) =>
            {
                services.AddSingleton<MainWindow>();
                services.AddTransient<LogInPage>();
                services.AddTransient<SignUpPage>();
                services.AddTransient<SetUpUserCredentialsPage>();
                services.AddTransient<OffersPage>();
                services.AddTransient<CreateOfferWindow>();
                services.AddTransient<OfferDetailsWindow>();
                services.AddTransient<UserAccountMenuContextWindow>();
                services.AddSingleton<UserAccountMenuNavigationPage>();
                services.AddTransient<UserAccountOffersPage>();
                services.AddTransient<UserAccountDetailsPage>();
                services.AddTransient<ChatWindow>();
                
                services.AddTransient<LogInViewModel>();
                services.AddTransient<SignUpViewModel>();
                services.AddTransient<SetUpUserCredentialsViewModel>();
                services.AddTransient<OffersViewModel>();
                services.AddTransient<CreateOfferViewModel>();
                services.AddTransient<OfferDetailsViewModel>();
                services.AddSingleton<UserAccountMenuNavigationViewModel>();
                services.AddTransient<UserAccountOffersViewModel>();
                services.AddTransient<UserAccountDetailsViewModel>();
                services.AddTransient<ChatViewModel>();
                
                services.AddSingleton<INavigationService, NavigationService>();
                services.AddSingleton<ISnackbarService, SnackbarService>();
                services.AddSingleton<IMapService, MapService>();
                services.AddSingleton<PostalAddressService>();
                services.AddSingleton<IApiClient, ApiClient>();
                services.AddSingleton<IChatHubClient, ChatHubClient>();
            })
            .Build();
    }

    private static IHost AppHost { get; set; } = null!;
    
    protected override async void OnStartup(StartupEventArgs e)
    {
        await AppHost.StartAsync();

        var startupWindow = AppHost.Services.GetRequiredService<MainWindow>();
        var logInPage = AppHost.Services.GetRequiredService<LogInPage>();
        startupWindow.MainFrame.Content = logInPage;
        startupWindow.Show();
        
        base.OnStartup(e);
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        var chatHubClient = AppHost.Services.GetRequiredService<IChatHubClient>();
        await chatHubClient.DisconnectAsync();
        
        await AppHost.StopAsync();
        
        base.OnExit(e);
    }
}