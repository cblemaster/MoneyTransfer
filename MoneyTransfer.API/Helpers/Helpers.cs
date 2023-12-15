namespace MoneyTransfer.API.Helpers
{
    public static class Helpers
    {
        public static bool StringIsValid(string eval, int minLength, int maxLength)
        {
            return !string.IsNullOrWhiteSpace(eval) &&
                eval != string.Empty &&
                eval.Length >= minLength &&
                eval.Length <= maxLength;
        }
    }
}
