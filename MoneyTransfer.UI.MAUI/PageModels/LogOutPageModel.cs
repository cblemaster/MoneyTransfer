using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MoneyTransfer.UI.MAUI.PageModels
{
    public partial class LogOutPageModel() : ObservableObject
    {
        [RelayCommand]
        private async Task LogOut()
        {
            //TODO: Logic to log out current logged in user
            await Shell.Current.GoToAsync("///LogIn");
        }
    }
}
