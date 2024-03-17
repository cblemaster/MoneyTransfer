using MoneyTransfer.Core.Validation;
using System.Text;

namespace MoneyTransfer.Core.DTO;

public class LogInUserDTO
{
    public required string Username { get; init; }
    public required string Password { get; init; }

    public ValidationResult Validate()
    {
        bool usernameIsValid = StringIsValid(Username, 50, 1);
        
        bool passwordIsValid = StringIsValid(Password, 200, 1);

        StringBuilder sb = new();

        if (!usernameIsValid)
        {
            sb.AppendLine("Username is required and must be 50 characters or fewer.");
        }
        if (!passwordIsValid)
        {
            sb.AppendLine("Password is required and must be 200 characters or fewer.");
        }

        bool isValid = usernameIsValid && passwordIsValid;
        string errorMessage = !isValid && sb.Length > 0 ? sb.ToString() : string.Empty;

        return new()
        {
            IsValid = isValid,
            ErrorMessage = errorMessage,
        };

        static bool StringIsValid(string username, int maxLength, int minLength)
        {
            return !string.IsNullOrWhiteSpace(username) &&
            username.Length >= minLength &&
            username.Length <= maxLength;
        }
    }

    
}
