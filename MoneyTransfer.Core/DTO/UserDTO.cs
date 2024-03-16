namespace MoneyTransfer.Core.DTO;

public class UserDTO
{
    public required int Id { get; init; }
    public required string Username { get; init; }
    public required string Token { get; init; }

    public static readonly UserDTO UserDTONotFound =
            new()
            {
                Id = 0,
                Username = "not found",
                Token = "not found",
            };

    public static readonly UserDTO UserDTONotValid =
        new()
        {
            Id = 0,
            Username = "not valid",
            Token = "not valid",
        };

    public static readonly UserDTO UserDTOUserNotAuthorized =
        new()
        {
            Id = 0,
            Username = "not authorized",
            Token = "not authorized",
        };

    public static readonly UserDTO UserDTOHttpResponseUnsuccessful =
        new()
        {
            Id = 0,
            Username = "http response unsuccessful",
            Token = "http response unsuccessful",
        };
}
