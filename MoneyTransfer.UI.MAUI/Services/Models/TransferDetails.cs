namespace MoneyTransfer.UI.MAUI.Services.Models
{
    public class TransferDetails(int id, decimal amount, string transferStatus, string transferType, DateOnly dateCreated, string userToName, string userFromName)
    {
        public int Id { get; } = id;

        public DateOnly DateCreated { get; } = dateCreated;

        public decimal Amount { get; } = amount;

        public string TransferStatus { get; } = transferStatus;

        public string TransferType { get; } = transferType;

        public string UserToName { get; } = userToName;

        public string UserFromName { get; } = userFromName;
    }
}
