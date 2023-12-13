using System.ComponentModel.DataAnnotations;

namespace MoneyTransfer.DataAccess
{
    public class Transfer(int id, DateOnly dateCreated, decimal amount, 
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
        public string TransferStatus { get; } = transferStatus;

        [Required(ErrorMessage = "Transfer Type is required.")]
        public string TransferType { get; } = transferType;

        [Required(ErrorMessage = "Transfer User To Name is required.")]
        public string UserToName { get; } = userToName;

        [Required(ErrorMessage = "Transfer User From Name is required.")]
        public string UserFromName { get; } = userFromName;

        public static readonly Transfer NotFound = new(id: 0, dateCreated: DateOnly.MinValue, amount: 0M, transferStatus: "not found", transferType: "not found", userFromName: "not found", userToName: "not found");
    }
}
