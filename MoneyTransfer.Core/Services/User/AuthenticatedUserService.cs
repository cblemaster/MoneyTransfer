using MoneyTransfer.Core.DTO;

namespace MoneyTransfer.UI.MAUI.Services.User
{
    public static class AuthenticatedUserService
    {
        private static UserDTO _user = default!;

        public static void LogOut() => _user = null!;

        public static string GetToken() => _user?.Token ?? string.Empty;

        public static int GetUserId() => _user is not null ? _user.Id : 0;

        public static bool IsLoggedIn() => _user is not null && !string.IsNullOrWhiteSpace(_user.Token);

        public static void SetLogin(UserDTO user) => _user = user;
    }
}
