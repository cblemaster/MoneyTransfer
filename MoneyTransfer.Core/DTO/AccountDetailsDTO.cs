namespace MoneyTransfer.Core.DTO;

public class AccountDetailsDTO
{
    public required int Id { get; init; }

    public required string Username { get; init; }

    public required decimal CurrentBalance { get; init; }

    public required DateOnly DateCreated { get; init; }

    public static readonly AccountDetailsDTO AccountNotFound =
            new()
            {
                Id = 0,
                Username = "not found",
                CurrentBalance = 0M,
                DateCreated = DateOnly.MinValue
            };

    public static readonly AccountDetailsDTO AccountUserNotAuthorized =
        new()
        {
            Id = 0,
            Username = "not authorized",
            CurrentBalance = 0M,
            DateCreated = DateOnly.MinValue
        };

    public static readonly AccountDetailsDTO AccountHttpResponseUnsuccessful =
        new()
        {
            Id = 0,
            Username = "http response unsuccessful",
            CurrentBalance = 0M,
            DateCreated = DateOnly.MinValue
        };
}
