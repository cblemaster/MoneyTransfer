namespace MoneyTransfer.UI.MAUI.Services.Data
{
    public class TransferDetails
    {
        public int Id { get; init; }

        public DateOnly DateCreated { get; init; }

        public decimal Amount { get; init; }

        public required string TransferStatus { get; init; }

        public required string TransferType { get; init; }

        public required string UserToName { get; init; }

        public required string UserFromName { get; init; }
    }
}
