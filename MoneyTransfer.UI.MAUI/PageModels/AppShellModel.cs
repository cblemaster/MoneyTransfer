using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using MoneyTransfer.UI.MAUI.Messages;

namespace MoneyTransfer.UI.MAUI.PageModels;

public partial class AppShellModel : ObservableObject
{
    [ObservableProperty]
    private bool _isUserLoggedIn;

    public AppShellModel() =>
        WeakReferenceMessenger.Default.Register<LoggedInUserChangedMessage>(this, (r, m) =>
            IsUserLoggedIn = m.Value);
}
