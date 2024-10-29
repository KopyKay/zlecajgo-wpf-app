using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ZlecajGoApi;
using ZlecajGoWpfApp.Services;
using ZlecajGoWpfApp.Views;
using ZlecajGoWpfApp.ViewModels;

namespace ZlecajGoWpfApp;

public partial class App : Application
{
    public static IHost AppHost { get; private set; } = null!;

    public App()
    {
        AppHost = Host.CreateDefaultBuilder()
            .ConfigureServices((hostContext, services) =>
            {
                services.AddSingleton<MainWindow>();
                services.AddSingleton<LogInPage>();
                services.AddSingleton<SignUpPage>();
                services.AddSingleton<SetUpUserCredentialsPage>();
                services.AddSingleton<LogInViewModel>();
                services.AddSingleton<SignUpViewModel>();
                services.AddSingleton<SetUpUserCredentialsViewModel>();
                services.AddSingleton<NavigationService>();
                services.AddSingleton<SnackbarService>();
                services.AddSingleton<IApiClient, ApiClient>();
            })
            .Build();
    }

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
        await AppHost.StopAsync();
        
        base.OnExit(e);
    }
}