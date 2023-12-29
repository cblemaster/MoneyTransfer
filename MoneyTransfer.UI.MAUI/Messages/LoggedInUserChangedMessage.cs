using CommunityToolkit.Mvvm.Messaging.Messages;
using MoneyTransfer.UI.MAUI.Services.User;

namespace MoneyTransfer.UI.MAUI.Messages
{
    public class LoggedInUserChangedMessage : ValueChangedMessage<bool>
    {
        public LoggedInUserChangedMessage(bool value) : base(value)
        {

        }
    }
}
