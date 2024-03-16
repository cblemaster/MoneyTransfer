namespace MoneyTransfer.UI.MAUI.Services.Data
{
    public class TransferDetails
    {
        public required int Id { get; init; }

        public required DateOnly DateCreated { get; init; }

        public required decimal Amount { get; init; }

        public required string TransferStatus { get; init; }

        public required string TransferType { get; init; }

        public required string UserToName { get; init; }

        public required string UserFromName { get; init; }
    }
}
