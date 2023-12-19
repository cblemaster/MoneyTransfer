namespace MoneyTransfer.UI.MAUI.Services.Models
{
    public class AccountDetails(int id, string username, decimal currentBalance, DateOnly dateCreated)
    {
        public int Id { get; } = id;

        public string Username { get; } = username;

        public decimal CurrentBalance { get; } = currentBalance;

        public DateOnly DateCreated { get; } = dateCreated;
    }
}
