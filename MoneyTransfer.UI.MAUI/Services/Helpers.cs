using MoneyTransfer.UI.MAUI.Services.Data;
using MoneyTransfer.UI.MAUI.Services.User;

namespace MoneyTransfer.UI.MAUI.Services
{
    public static class Helpers
    {
        public static bool StringIsValid(string eval, int minLength, int maxLength) =>
            !string.IsNullOrWhiteSpace(eval) &&
            eval.Length >= minLength &&
            eval.Length <= maxLength;

        public static bool AddTransferIsValid(AddTransfer addTransfer)
        {
            bool amountIsValid = addTransfer.Amount > 0;
            bool userFromNameIsValid = StringIsValid(addTransfer.UserFromName, 1, 50);
            bool userToNameIsValid = StringIsValid(addTransfer.UserToName, 1, 50);
            bool userToAndUserFromAreNotTheSame = !addTransfer.UserToName.Equals(addTransfer.UserFromName);

            return amountIsValid && userFromNameIsValid &&
                userToNameIsValid && userToAndUserFromAreNotTheSame;
        }

        public static bool LogInUserIsValid(LogInUser logInUser)
        {
            bool usernameIsValid = !string.IsNullOrWhiteSpace(logInUser.Username) &&
                logInUser.Username.Length >= 1 &&
                logInUser.Username.Length <= 50;
            bool passwordIsValid = !string.IsNullOrWhiteSpace(logInUser.Password) &&
                logInUser.Password.Length >= 1 &&
                logInUser.Password.Length <= 200;
            return usernameIsValid && passwordIsValid;
        }

        public static readonly AccountDetails AccountNotFound =
            new()
            {
                Id = 0,
                Username = "not found",
                CurrentBalance = 0M,
                DateCreated = DateOnly.MinValue
            };

        public static readonly AccountDetails AccountUserNotAuthorized =
            new()
            {
                Id = 0,
                Username = "not authorized",
                CurrentBalance = 0M,
                DateCreated = DateOnly.MinValue
            };

        public static readonly AccountDetails AccountHttpResponseUnsuccessful =
            new()
            {
                Id = 0,
                Username = "http response unsuccessful",
                CurrentBalance = 0M,
                DateCreated = DateOnly.MinValue
            };

        public static readonly TransferDetails TransferNotFound =
            new()
            {
                Id = 0,
                Amount = 0M,
                TransferStatus = "not found",
                TransferType = "not found",
                DateCreated = DateOnly.MinValue,
                UserToName = "not found",
                UserFromName = "not found",
            };

        public static readonly TransferDetails TransferUserNotAuthorized =
            new()
            {
                Id = 0,
                Amount = 0M,
                TransferStatus = "not authorized",
                TransferType = "not authorized",
                DateCreated = DateOnly.MinValue,
                UserToName = "not authorized",
                UserFromName = "not authorized",
            };

        public static readonly TransferDetails TransferHttpResponseUnsuccessful =
            new()
            {
                Id = 0,
                Amount = 0M,
                TransferStatus = "http response unsuccessful",
                TransferType = "http response unsuccessful",
                DateCreated = DateOnly.MinValue,
                UserToName = "http response unsuccessful",
                UserFromName = "http response unsuccessful",
            };

        public static readonly UserDTO UserDTONotFound =
            new()
            {
                Id = 0,
                Username = "not found",
                Token = "not found",
            };

        public static readonly UserDTO UserDTONotValid =
            new()
            {
                Id = 0,
                Username = "not valid",
                Token = "not valid",
            };

        public static readonly UserDTO UserDTOUserNotAuthorized =
            new()
            {
                Id = 0,
                Username = "not authorized",
                Token = "not authorized",
            };

        public static readonly UserDTO UserDTOHttpResponseUnsuccessful =
            new()
            {
                Id = 0,
                Username = "http response unsuccessful",
                Token = "http response unsuccessful",
            };

        public static readonly User.User UserNotFound =
             new()
             {
                 Id = 0,
                 Username = "not found",
             };
    }
}
