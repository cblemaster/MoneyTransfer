namespace MoneyTransfer.UI.MAUI.Services.Models
{
    public static class Helpers
    {
        public static bool StringIsValid(string eval, int minLength, int maxLength) =>
            !string.IsNullOrWhiteSpace(eval) &&
            eval != string.Empty &&
            eval.Length >= minLength &&
            eval.Length <= maxLength;
    }
}
