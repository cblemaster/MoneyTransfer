using System.ComponentModel.DataAnnotations;
using Helper = MoneyTransfer.API.Helpers.Helpers;

namespace MoneyTransfer.API.Entities
{
    public class Account(int id, string username, decimal balance)
    {
        [Key]
        [Required(ErrorMessage = "Account Id is required.")]
        public int Id { get; } = id;

        [Required(ErrorMessage = "Account Username is required.")]
        [MaxLength(50, ErrorMessage = "Max length for Username is 50")]
        public string Username { get; } = username;

        [Required(ErrorMessage = "Account Balance is required.")]
        public decimal Balance { get; } = balance;

        public static readonly Account NotFound = new(id: 0, username: "not found", balance: 0M);

        public bool IsValid()
        {
            bool idIsValid = Id > 0;
            bool usernameIsValid = Helper.StringIsValid(Username, 1, 50);
            bool balanceIsValid = Balance > 0;

            return idIsValid && usernameIsValid &&
                balanceIsValid;
        }
    }
}
