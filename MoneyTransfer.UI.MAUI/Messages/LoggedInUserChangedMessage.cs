using CommunityToolkit.Mvvm.Messaging.Messages;

namespace MoneyTransfer.UI.MAUI.Messages
{
    public class LoggedInUserChangedMessage(bool value) : ValueChangedMessage<bool>(value)
    {
    }
}
