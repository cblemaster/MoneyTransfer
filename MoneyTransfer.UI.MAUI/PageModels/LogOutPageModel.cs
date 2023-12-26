using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MoneyTransfer.UI.MAUI.Services.User;

namespace MoneyTransfer.UI.MAUI.PageModels
{
    public partial class LogOutPageModel() : ObservableObject
    {
        [RelayCommand]
        private async Task LogOut()
        {
            if (!CanLogOut) { return; }
            HttpUserService.LogOut();
            await Shell.Current.GoToAsync("///LogIn");
        }

        [ObservableProperty]
        private bool _canLogOut = true;
    }
}
