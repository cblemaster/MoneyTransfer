namespace MoneyTransfer.API.Entities;

public partial class User
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string Salt { get; set; } = null!;

    public virtual Account? Account { get; set; }

    public static readonly User NotFound = new() { Id = 0, Username = "not found", PasswordHash = "not found", Salt = "not found", Account = null };
}

public class ReturnUser
{
    public int Id { get; set; }
    public required string Username { get; set; }
    public required string Token { get; set; }
}

public class LogInUser
{
    public required string Username { get; set; }
    public required string Password { get; set; }
}
