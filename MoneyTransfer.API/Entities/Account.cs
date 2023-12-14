using System.ComponentModel.DataAnnotations;

namespace MoneyTransfer.API.Entities
{
    public class Account(int id, string username, decimal balance)
    {
        [Key]
        [Required(ErrorMessage = "Account Id is required.")]
        public int Id { get; } = id;

        [Required(ErrorMessage = "Account Username is required.")]
        public string Username { get; } = username;

        [Required(ErrorMessage = "Account Balance is required.")]
        public decimal Balance { get; } = balance;

        public static readonly Account NotFound = new(id: 0, username: "not found", balance: 0M);
    }
}
