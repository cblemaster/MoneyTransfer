using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MoneyTransfer.UI.MAUI.Services;
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
        private void PageAppearing()
        {
            Username = string.Empty;
            Password = string.Empty;

            CanRegister = !AuthenticatedUserService.IsLoggedIn();
        }

        [RelayCommand]
        private async Task Register()
        {
            if (!CanRegister) { return; }

            LogInUser logInUser = new() { Username = Username, Password = Password };
            if (!Helpers.LogInUserIsValid(logInUser)) { return; }

            bool isRegistered = await _userService.Register(new LogInUser { Username = Username, Password = Password });
            if (isRegistered)
            {
                await Shell.Current.DisplayAlert("Success!", "You have been registered, and you will be directed to the Log In page.", "OK");
                await Shell.Current.GoToAsync("///LogIn");
            }
        }

        [ObservableProperty]
        private bool _canRegister = !AuthenticatedUserService.IsLoggedIn();
    }
}
