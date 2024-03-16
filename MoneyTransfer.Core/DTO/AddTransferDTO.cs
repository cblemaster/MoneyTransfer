namespace MoneyTransfer.Core.DTO;

public class AddTransferDTO
{
    public required string UserFromName { get; set; } = string.Empty;

    public required string UserToName { get; set; } = string.Empty;

    public required decimal Amount { get; set; }

    public bool IsValid =>
        UsernameIsValid(UserFromName) &&
        UsernameIsValid(UserToName) &&
        Amount > 0 &&
        UserFromName != UserToName;

    public bool UsernameIsValid(string username) =>
        !string.IsNullOrEmpty(username) &&
        !string.IsNullOrWhiteSpace(username) &&
        username.Length > 0 &&
        username.Length <= 50;
}
