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

    public static Account NotFound => new() { Id = 0, StartingBalance = 0M, TransferAccountIdFromNavigations = null!, TransferAccountIdToNavigations = null!, User = null!, UserId = 0, DateCreated = DateOnly.MinValue };

    public decimal CurrentBalance()
    {
        return StartingBalance +
            TransferAccountIdToNavigations
                .Where(transfer => transfer.TransferStatus ==
                                    TransferStatus.Approved)
                .Sum(transfer => transfer.Amount) -
            TransferAccountIdFromNavigations
                .Where(transfer => transfer.TransferStatus ==
                                    TransferStatus.Approved)
                .Sum(transfer => transfer.Amount);
    }

    public bool IsValid() => UserId > 0;
}
