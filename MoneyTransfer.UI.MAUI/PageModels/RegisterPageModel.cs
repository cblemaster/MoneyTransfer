using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MoneyTransfer.Core.DTO;
using MoneyTransfer.Core.Services.User;

namespace MoneyTransfer.UI.MAUI.PageModels;

public partial class RegisterPageModel(IUserService userService) : ObservableObject
{
    private readonly IUserService _userService = userService;

    [ObservableProperty]
    private string _username = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

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

        LogInUserDTO logInUser = new() { Username = Username, Password = Password };
        if (!logInUser.Validate().IsValid) { return; }

        bool isRegistered = await _userService.Register(new LogInUserDTO { Username = Username, Password = Password });
        if (isRegistered)
        {
            await Shell.Current.DisplayAlert("Success!", "You have been registered, and you will be directed to the Log In page.", "OK");
            await Shell.Current.GoToAsync("///LogIn");
        }
    }

    [ObservableProperty]
    private bool _canRegister = !AuthenticatedUserService.IsLoggedIn();
}
