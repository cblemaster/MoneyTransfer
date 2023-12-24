using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MoneyTransfer.UI.MAUI.PageModels
{
    public partial class LogInPageModel() : ObservableObject
    {
        [ObservableProperty]
        private string _username = default!;

        [ObservableProperty]
        private string _password = default!;

        [RelayCommand]
        private void LogIn()
        {
            if (!CanLogIn) { return; }
            // TODO: Logic for logging in a user
        }

        [ObservableProperty]
        private bool _canLogIn = true;
    }
}
