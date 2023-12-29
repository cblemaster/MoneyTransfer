﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MoneyTransfer.UI.MAUI.Messages;
using MoneyTransfer.UI.MAUI.Services.User;

namespace MoneyTransfer.UI.MAUI.PageModels
{
    public partial class LogOutPageModel() : ObservableObject
    {
        [RelayCommand]
        private async Task LogOut()
        {
            if (!CanLogOut) { return; }

            AuthenticatedUserService.LogOut();
            WeakReferenceMessenger.Default.Send(new LoggedInUserChangedMessage(AuthenticatedUserService.IsLoggedIn()));

            await Shell.Current.DisplayAlert("Success!", "You are logged out of the system, and you will be directed to the Log In page.", "OK");
            await Shell.Current.GoToAsync("///LogIn");
        }

        [ObservableProperty]
        private bool _canLogOut = AuthenticatedUserService.IsLoggedIn();
    }
}
