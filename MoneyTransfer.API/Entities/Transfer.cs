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

    public TransferStatus TransferStatus => (TransferStatus)TransferStatusId;

    public TransferType TransferType => (TransferType)TransferTypeId;

    public bool IsValid =>
        TransferTypeId > 0 &&
        TransferStatusId > 0 &&
        AccountIdFrom > 0 &&
        AccountIdTo > 0 &&
        Amount > 0 &&
        AccountIdFrom != AccountIdTo;

    public bool IsValidForAdd => IsValid && Id == 0;

    public bool IsValidForUpdate => IsValid && Id > 0;
    
    public bool IsValidForApproveOrReject => IsValidForUpdate && TransferStatus == TransferStatus.Pending;
}
