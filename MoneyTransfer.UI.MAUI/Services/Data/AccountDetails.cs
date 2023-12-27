namespace MoneyTransfer.UI.MAUI.Services.Data
{
    public class AccountDetails
    {
        public required int Id { get; init; }

        public required string Username { get; init; }

        public required decimal CurrentBalance { get; init; }

        public required DateOnly DateCreated { get; init; }
    }
}
