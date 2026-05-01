using ZlecajGoWpfApp.CustomControls;
using ZlecajGoWpfApp.Enums;

namespace ZlecajGoWpfApp.UnitTests.Helpers;

public sealed class CustomMessageBoxScope : IDisposable
{
    public string? LastMessage { get; private set; }
    public CustomMessageBoxType? LastType { get; private set; }
    public string? LastTitle { get; private set; }

    public CustomMessageBoxScope()
    {
        CustomMessageBox.ShowHandler = (message, type, title) =>
        {
            LastMessage = message;
            LastType = type;
            LastTitle = title;
        };
    }

    public void Dispose()
    {
        CustomMessageBox.ShowHandler = null;
    }
}