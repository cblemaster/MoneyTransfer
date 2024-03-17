using MoneyTransfer.Core.Validation;

namespace MoneyTransfer.Core.Entities;

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

    public ValidationResult Validate()
    {
        bool isValid = TransferTypeId > 0 && TransferStatusId > 0 && AccountIdFrom > 0
            && AccountIdTo > 0 && Amount > 0 && AccountIdFrom != AccountIdTo;

        string errorMessage = "One or more invalid values for this transfer.";

        return new() { IsValid = isValid, ErrorMessage = !isValid ? errorMessage : string.Empty };
    }

    public bool IsValidForAdd => Validate().IsValid && Id.Equals(0);

    public bool IsValidForUpdate => Validate().IsValid && Id > 0;

    public bool IsValidForApproveOrReject => IsValidForUpdate && TransferStatus.Equals(Entities.TransferStatus.Pending.ToString());
}
