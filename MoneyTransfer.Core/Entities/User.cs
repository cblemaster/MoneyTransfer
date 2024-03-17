namespace MoneyTransfer.Core.Entities;

public partial class User
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string Salt { get; set; } = null!;

    public virtual Account? Account { get; set; }

    public static readonly User NotFound = new() { Id = 0, Username = "not found", PasswordHash = "not found", Salt = "not found", Account = null };
}

