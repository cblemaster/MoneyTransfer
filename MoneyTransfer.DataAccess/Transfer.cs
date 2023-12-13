using System.ComponentModel.DataAnnotations;

namespace MoneyTransfer.DataAccess
{
    public class Transfer
    {
        [Key]
        [Required(ErrorMessage = "Transfer Id is required.")]
        public int Id { get; }

        [Required(ErrorMessage = "Transfer Date Created is required.")]
        public DateOnly DateCreated { get; }

        [Required(ErrorMessage = "Transfer Amount is required.")]
        public decimal Amount { get; }

        [Required(ErrorMessage = "Transfer Status is required.")]
        public string TransferStatus { get; } = string.Empty;

        [Required(ErrorMessage = "Transfer Type is required.")]
        public string TransferType { get; } = string.Empty;

        [Required(ErrorMessage = "Transfer User To Name is required.")]
        public string UserToName { get; } = string.Empty;

        [Required(ErrorMessage = "Transfer User From Name is required.")]
        public string UserFromName { get; } = string.Empty;

        public Transfer(int id, DateOnly dateCreated, decimal amount, string transferStatus, string transferType, string userToName, string userFromName)
        {
            Id = id;
            DateCreated = dateCreated;
            Amount = amount;
            TransferStatus = transferStatus;
            TransferType = transferType;
            UserToName = userToName;
            UserFromName = userFromName;
        }
        
        public static Transfer NotFound = new Transfer(id: 0, dateCreated: DateOnly.MinValue, amount: 0M, transferStatus: "not found", transferType: "not found", userFromName: "not found", userToName: "not found");
    }
}
