using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MoneyTransfer.UI.MAUI.Services;
using System.Security;

namespace MoneyTransfer.UI.MAUI.PageModels
{
    public partial class RegisterPageModel() : ObservableObject
    {
        [ObservableProperty]
        private string _username = default!;

        [ObservableProperty]
        private string _password = default!;

        [RelayCommand]
        private void Register()
        {
            if (!CanRegister) { return; }
            // TODO: Logic for registering a new user
        }

        [ObservableProperty]
        private bool _canRegister = true;
    }
}
