namespace MoneyTransfer.API.Entities;

public partial class Transfer
{
    public int Id { get; set; }

    public int TransferTypeId { get; set; }

    public int TransferStatusId { get; set; }

    public int AccountIdFrom { get; set; }

    public int AccountIdTo { get; set; }

    public decimal Amount { get; set; }

    public DateOnly DateCreated { get; set; }

    public virtual Account AccountIdFromNavigation { get; set; } = null!;

    public virtual Account AccountIdToNavigation { get; set; } = null!;

    public TransferType TransferStatus => (TransferType)TransferStatusId;
    
    public TransferType TransferType => (TransferType)TransferTypeId;
}
