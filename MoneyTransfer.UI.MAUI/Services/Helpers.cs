using MoneyTransfer.UI.MAUI.Services.Data;
using MoneyTransfer.UI.MAUI.Services.User;

namespace MoneyTransfer.UI.MAUI.Services
{
    public static class Helpers
    {
        public static bool StringIsValid(string eval, int minLength, int maxLength) =>
            !string.IsNullOrWhiteSpace(eval) &&
            eval != string.Empty &&
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

        public static readonly AccountDetails AccountNotFound =
            new()
            {
                Id = 0,
                Username = "not found",
                CurrentBalance = 0M,
                DateCreated = DateOnly.MinValue
            };

        public static readonly AccountDetails AccountSearchParamNotValid =
            new()
            {
                Id = 0,
                Username = "search param not valid",
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
                UserFromName = "not found"
            };

        public static readonly TransferDetails TransferSearchParamNotValid =
            new()
            {
                Id = 0,
                Amount = 0M,
                TransferStatus = "search param not valid",
                TransferType = "search param not valid",
                DateCreated = DateOnly.MinValue,
                UserToName = "search param not valid",
                UserFromName = "search param not valid"
            };

        public static readonly UserDTO UserDTONotFound =
            new()
            {
                Id = 0,
                Username = "not found",
                Token = "not found",
                Message = "not found",
            };

        public static readonly UserDTO UserDTOSearchParamNotValid =
            new()
            {
                Id = 0,
                Username = "search param not valid",
                Token = "search param not valid",
                Message = "search param not valid",
            };

        public static readonly User.User UserNotFound =
             new()
             {
                 Id = 0,
                 Username = "not found",
             };

        public static readonly User.User UserSearchParamNotValid =
            new()
            {
                Id = 0,
                Username = "search param not valid",
            };
    }
}
