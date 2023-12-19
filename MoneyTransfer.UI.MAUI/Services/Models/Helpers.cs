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
    }
}
