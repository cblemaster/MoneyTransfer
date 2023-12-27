﻿namespace MoneyTransfer.UI.MAUI.Services.User
{
    public static class AuthenticatedUserService
    {
        private static UserDTO _user = default!;
        public static void LogOut() => _user = null!;

        public static string GetToken() => _user?.Token ?? string.Empty;

        public static int GetUserId() => _user.Id;

        public static bool IsLoggedIn() => !string.IsNullOrWhiteSpace(_user.Token);

        public static void SetLogin(UserDTO user) => _user = user;
    }
}
