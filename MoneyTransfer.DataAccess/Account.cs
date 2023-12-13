using System.ComponentModel.DataAnnotations;

namespace MoneyTransfer.DataAccess
{
    public class Account
    {
        [Key]
        [Required(ErrorMessage = "Account Id is required.")]
        public int Id { get; }

        [Required(ErrorMessage = "Account Username is required.")]
        public string Username { get;} = string.Empty;

        [Required(ErrorMessage = "Account Balance is required.")]
        public decimal Balance { get; }

        public Account(int id, string username, decimal balance)
        {
            Id = id;
            Username = username;
            Balance = balance;
        }

        public static Account NotFound = new Account(id: 0, username: "not found", balance: 0M);
    }
}
