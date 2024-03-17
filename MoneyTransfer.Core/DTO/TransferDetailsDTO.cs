using MoneyTransfer.Core.Validation;

namespace MoneyTransfer.Core.DTO;

public class TransferDetailsDTO
{
    public required int Id { get; init; }

    public required DateOnly DateCreated { get; init; }

    public required decimal Amount { get; init; }

    public required string TransferStatus { get; init; }

    public required string TransferType { get; init; }

    public required string UserToName { get; init; }

    public required string UserFromName { get; init; }

    public static readonly TransferDetailsDTO TransferNotFound =
            new()
            {
                Id = 0,
                Amount = 0M,
                TransferStatus = "not found",
                TransferType = "not found",
                DateCreated = DateOnly.MinValue,
                UserToName = "not found",
                UserFromName = "not found",
            };

    public static readonly TransferDetailsDTO TransferUserNotAuthorized =
        new()
        {
            Id = 0,
            Amount = 0M,
            TransferStatus = "not authorized",
            TransferType = "not authorized",
            DateCreated = DateOnly.MinValue,
            UserToName = "not authorized",
            UserFromName = "not authorized",
        };

    public static readonly TransferDetailsDTO TransferHttpResponseUnsuccessful =
        new()
        {
            Id = 0,
            Amount = 0M,
            TransferStatus = "http response unsuccessful",
            TransferType = "http response unsuccessful",
            DateCreated = DateOnly.MinValue,
            UserToName = "http response unsuccessful",
            UserFromName = "http response unsuccessful",
        };

    public ValidationResult Validate()
    {
        bool isValid = TransferType.Length > 0 && TransferStatus.Length > 0 &&
        UserFromName.Length > 0 && UserToName.Length > 0 && Amount > 0M;

        string errorMessage = "One or more invalid values for this transfer.";

        return new() { IsValid = isValid, ErrorMessage = !isValid ? errorMessage : string.Empty };
    }

    public bool IsValidForAdd => Validate().IsValid && Id.Equals(0);

    public bool IsValidForUpdate => Validate().IsValid && Id > 0;

    public bool IsValidForApproveOrReject => IsValidForUpdate && TransferStatus.Equals(Entities.TransferStatus.Pending.ToString());
}

