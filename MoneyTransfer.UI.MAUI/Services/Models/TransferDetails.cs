using System.ComponentModel.DataAnnotations;
using Helper = MoneyTransfer.UI.MAUI.Services.Models.Helpers;

namespace MoneyTransfer.UI.MAUI.Services.Models
{
    public class TransferDetails(int id, DateOnly dateCreated, decimal amount,
        string transferStatus, string transferType, string userToName,
        string userFromName)
    {
        [Key]
        [Required(ErrorMessage = "Transfer Id is required.")]
        public int Id { get; } = id;

        [Required(ErrorMessage = "Transfer Date Created is required.")]
        public DateOnly DateCreated { get; } = dateCreated;

        [Required(ErrorMessage = "Transfer Amount is required.")]
        public decimal Amount { get; } = amount;

        [Required(ErrorMessage = "Transfer Status is required.")]
        [MaxLength(10, ErrorMessage = "Max length for Transfer Status is 10")]
        public string TransferStatus { get; } = transferStatus;

        [Required(ErrorMessage = "Transfer Type is required.")]
        [MaxLength(10, ErrorMessage = "Max length for Transfer Type is 10")]
        public string TransferType { get; } = transferType;

        [Required(ErrorMessage = "Transfer User To Name is required.")]
        [MaxLength(50, ErrorMessage = "Max length for User To Name is 50")]
        public string UserToName { get; } = userToName;

        [Required(ErrorMessage = "Transfer User From Name is required.")]
        [MaxLength(50, ErrorMessage = "Max length for User From Name is 50")]
        public string UserFromName { get; } = userFromName;

        public static readonly TransferDetails NotFound = new(id: 0, dateCreated: DateOnly.MinValue, amount: 0M, transferStatus: "not found", transferType: "not found", userFromName: "not found", userToName: "not found");

        public bool IsValid()
        {
            bool idIsValid = Id > 0;
            bool amountIsValid = Amount > 0;
            bool transferTypeIsValid = Helper.StringIsValid(TransferType, 1, 10);
            bool transferStatusIsValid = Helper.StringIsValid(TransferStatus, 1, 10);
            bool userFromNameIsValid = Helper.StringIsValid(UserFromName, 1, 50);
            bool userToNameIsValid = Helper.StringIsValid(UserToName, 1, 50);
            bool userToAndUserFromAreNotTheSame = !UserToName.Equals(UserFromName);

            return idIsValid && amountIsValid &&
                transferTypeIsValid && transferStatusIsValid &&
                userFromNameIsValid && userToNameIsValid &&
                userToAndUserFromAreNotTheSame;
        }
    }
}
