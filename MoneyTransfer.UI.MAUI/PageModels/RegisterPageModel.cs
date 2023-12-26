using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MoneyTransfer.UI.MAUI.Services.User;

namespace MoneyTransfer.UI.MAUI.PageModels
{
    public partial class RegisterPageModel(IUserService userService) : ObservableObject
    {
        private readonly IUserService _userService = userService;

        [ObservableProperty]
        private string _username = default!;

        [ObservableProperty]
        private string _password = default!;

        [RelayCommand]
        private async Task Register()
        {
            if (!CanRegister) { return; }
            bool isRegistered = await _userService.Register(new LogInUser { Username = Username, Password = Password });
            if (isRegistered) { await Shell.Current.GoToAsync("///LogIn"); }
        }

        [ObservableProperty]
        private bool _canRegister = true;
    }
}
