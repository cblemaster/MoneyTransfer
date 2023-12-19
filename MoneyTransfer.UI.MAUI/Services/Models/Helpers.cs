namespace MoneyTransfer.UI.MAUI.Services.Models
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
            new(id: 0, username: "not found", currentBalance: 0M, dateCreated: DateOnly.MinValue);

        public static readonly AccountDetails AccountSearchParamNotValid =
            new(id: 0, username: "search param not valid", currentBalance: 0M, dateCreated: DateOnly.MinValue);

        public static readonly TransferDetails TransferNotFound = 
            new(id: 0, amount: 0M, transferStatus: "not found", transferType: "not found", 
                dateCreated: DateOnly.MinValue, userToName: "not found", userFromName: "not found");

        public static readonly TransferDetails TransferSearchParamNotValid =
            new(id: 0, amount: 0M, transferStatus: "search param not valid", transferType: "search param not valid",
                dateCreated: DateOnly.MinValue, userToName: "search param not valid", userFromName: "search param not valid");
    }
}
