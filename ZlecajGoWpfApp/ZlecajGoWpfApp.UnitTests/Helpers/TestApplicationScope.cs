using System.Windows;
using System.Windows.Media;

namespace ZlecajGoWpfApp.UnitTests.Helpers;

public sealed class TestApplicationScope : IDisposable
{
    private static bool _initialized;

    public TestApplicationScope()
    {
        var app = Application.Current;
        if (app == null)
        {
            app = new Application
            {
                ShutdownMode = ShutdownMode.OnExplicitShutdown
            };

            _initialized = true;
        }

        app.Resources["AccentColorBrush1"] = Brushes.Blue;
        app.Resources["AccentColorBrush2"] = Brushes.Green;
    }

    public void Dispose()
    {
        if (_initialized && Application.Current != null)
        {
            // Keep the application alive for the AppDomain lifetime to avoid WPF reinit issues.
        }
    }
}