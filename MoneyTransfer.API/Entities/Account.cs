namespace MoneyTransfer.API.Entities;

public partial class Account
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public decimal StartingBalance { get; set; }

    public DateOnly DateCreated { get; set; }

    public virtual ICollection<Transfer> TransferAccountIdFromNavigations { get; set; } = new List<Transfer>();

    public virtual ICollection<Transfer> TransferAccountIdToNavigations { get; set; } = new List<Transfer>();

    public virtual User User { get; set; } = null!;

    public decimal CurrentBalance() =>
        StartingBalance +
        TransferAccountIdToNavigations
            .Where(transfer => transfer.TransferStatus == TransferStatus.Approved)
            .Sum(transfer => transfer.Amount) -
        TransferAccountIdFromNavigations
            .Where(transfer => transfer.TransferStatus == TransferStatus.Approved)
            .Sum(transfer => transfer.Amount);
}
