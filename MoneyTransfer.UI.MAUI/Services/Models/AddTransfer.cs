using System.ComponentModel.DataAnnotations;
using Helper = MoneyTransfer.UI.MAUI.Services.Models.Helpers;

namespace MoneyTransfer.UI.MAUI.Services.Models
{
    public class AddTransfer
    {
        [Required(ErrorMessage = "Transfer User From Name is required.")]
        [MaxLength(50, ErrorMessage = "Max length for User From Name is 50.")]
        public string UserFromName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Transfer User To Name is required.")]
        [MaxLength(50, ErrorMessage = "Max length for User To Name is 50.")]
        public string UserToName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Transfer Amount is required.")]
        public decimal Amount { get; set; }

        public bool IsValid()
        {
            bool amountIsValid = Amount > 0;
            bool userFromNameIsValid = Helper.StringIsValid(UserFromName, 1, 50);
            bool userToNameIsValid = Helper.StringIsValid(UserToName, 1, 50);
            bool userToAndUserFromAreNotTheSame = !UserToName.Equals(UserFromName);

            return amountIsValid && userFromNameIsValid &&
                userToNameIsValid && userToAndUserFromAreNotTheSame;
        }
    }
}
