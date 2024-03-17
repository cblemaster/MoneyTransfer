using MoneyTransfer.Core.Validation;
using System.Text;

namespace MoneyTransfer.Core.DTO;

public class AddTransferDTO
{
    public required string UserFromName { get; set; } = string.Empty;

    public required string UserToName { get; set; } = string.Empty;

    public required decimal Amount { get; set; }

    public ValidationResult Validate()
    {
        bool userFromIsValid = UsernameIsValid(UserFromName);
        bool userToIsValid = UsernameIsValid(UserToName);
        bool amountIsValid = Amount > 0;

        StringBuilder sb = new();

        if (!userFromIsValid)
        {
            sb.AppendLine("The user from name is required and must be 50 characters or fewer.");
        }
        if (!userToIsValid)
        {
            sb.AppendLine("The user to name is required and must be 50 characters or fewer.");
        }
        if (UserFromName.Equals(UserToName))
        {
            sb.AppendLine("User from and user to cannot be the same.");
        }
        if (!amountIsValid)
        {
            sb.AppendLine("Amount must be greater than zero.");
        }

        bool isValid = userFromIsValid && userToIsValid && amountIsValid;
        string errorMessage = !isValid && sb.Length > 0 ? sb.ToString() : string.Empty;

        return new()
        {
            IsValid = isValid,
            ErrorMessage = errorMessage,
        };

        bool UsernameIsValid(string username) =>
            !string.IsNullOrWhiteSpace(username) &&
            username.Length >= 1 &&
            username.Length <= 50;
    }
}
