﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MoneyTransfer.UI.MAUI.Messages;
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
        private void PageAppearing()
        {
            Username = string.Empty;
            Password = string.Empty;

            CanLogIn = !AuthenticatedUserService.IsLoggedIn();
        }

        [RelayCommand]
        private async Task LogIn()
        {
            if (!CanLogIn) { return; }

            LogInUser logInUser = new() { Username = Username, Password = Password };
            if (!Helpers.LogInUserIsValid(logInUser)) { return; }

            UserDTO loggedInUser = await _userService.LogIn(logInUser);
            if (loggedInUser.Id > 0)
            {
                AuthenticatedUserService.SetLogin(loggedInUser);
                WeakReferenceMessenger.Default.Send(new LoggedInUserChangedMessage(AuthenticatedUserService.IsLoggedIn()));

                await Shell.Current.DisplayAlert("Success!", "You are logged into the system.", "OK");

                await Shell.Current.GoToAsync("///AccountDetails");
            }
        }

        [ObservableProperty]
        private bool _canLogIn = !AuthenticatedUserService.IsLoggedIn();
    }
}
