namespace MoneyTransfer.Core.DTO;

public class LogInUserDTO
{
    public required string Username { get; init; }
    public required string Password { get; init; }

    public bool IsValid()
    {
        bool usernameIsValid = !string.IsNullOrWhiteSpace(Username) &&
            Username.Length >= 1 &&
            Username.Length <= 50;
        
        bool passwordIsValid = !string.IsNullOrWhiteSpace(Password) &&
            Password.Length >= 1 &&
            Password.Length <= 200;
        
        return usernameIsValid && passwordIsValid;
    }
}
