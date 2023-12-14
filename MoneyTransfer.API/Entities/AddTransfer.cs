using System.ComponentModel.DataAnnotations;

namespace MoneyTransfer.API.Entities
{
    public class AddTransfer
    {
        [Required(ErrorMessage = "User From Name is required.")]
        [MaxLength(50, ErrorMessage = "Max length for User From Name is 50.")]
        public string UserFromName { get; set; } = string.Empty;

        [Required(ErrorMessage = "User To Name is required.")]
        [MaxLength(50, ErrorMessage = "Max length for User To Name is 50.")]
        public string UserToName { get; set; } = string.Empty;

        [Required]
        public decimal Amount { get; set; }

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(UserToName) &&
                !string.IsNullOrWhiteSpace(UserToName) &&
                UserToName.Length > 0 && UserToName.Length <= 50 &&
                !string.IsNullOrEmpty(UserFromName) &&
                !string.IsNullOrWhiteSpace(UserFromName) &&
                UserFromName.Length > 0 && UserFromName.Length <= 50 &&
                UserFromName != UserToName &&
                Amount > 0;
        }
    }
}
