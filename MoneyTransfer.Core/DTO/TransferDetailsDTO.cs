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
}
