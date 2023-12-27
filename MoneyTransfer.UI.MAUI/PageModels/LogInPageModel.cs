﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MoneyTransfer.UI.MAUI.Services;
using MoneyTransfer.UI.MAUI.Services.User;

namespace MoneyTransfer.UI.MAUI.PageModels
{
    public partial class LogInPageModel(IUserService userService) : ObservableObject
    {
        private readonly IUserService _userService = userService;

        [ObservableProperty]
        private string _username = default!;

        [ObservableProperty]
        private string _password = default!;

        [RelayCommand]
        private async Task LogIn()
        {
            if (!CanLogIn) { return; }

            LogInUser logInUser = new LogInUser { Username = Username, Password = Password };
            if (!Helpers.LogInUserIsValid(logInUser)) { return; }

            UserDTO loggedInUser = await _userService.LogIn(logInUser);
            AuthenticatedUserService.SetLogin(loggedInUser);
            await Shell.Current.DisplayAlert("Success!", "You are logged into the system.", "OK");

            Username = string.Empty;
            Password = string.Empty;
        }

        [ObservableProperty]
        private bool _canLogIn = true;
    }
}
