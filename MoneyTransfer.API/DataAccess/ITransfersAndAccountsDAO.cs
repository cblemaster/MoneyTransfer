using MoneyTransfer.API.Entities;

namespace MoneyTransfer.API.DataAccess
{
    public interface ITransfersAndAccountsDAO
    {
        Task ApproveTransferRequestAsync(int transferId);
        Task<Account> GetAccountDetailsForUserAsync(string username);
        Task<List<Transfer>> GetCompletedTransfersForUserAsync(string username);
        Task<List<Transfer>> GetPendingTransfersForUserAsync(string username);
        Task<Transfer> GetTransferDetailsAsync(int transferId);
        Task RejectTransferRequestAsync(int transferId);
        Task RequestTransferAsync(string userFromName, string userToName, decimal amount);
        Task SendTransferAsync(string userFromName, string userToName, decimal amount);
    }
}
