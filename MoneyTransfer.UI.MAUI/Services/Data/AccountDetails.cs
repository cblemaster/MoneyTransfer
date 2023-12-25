namespace MoneyTransfer.UI.MAUI.Services.Data
{
    public class AccountDetails
    {
        public int Id { get; init; }

        public required string Username { get; init; }

        public decimal CurrentBalance { get; init; }

        public DateOnly DateCreated { get; init; }
    }
}
