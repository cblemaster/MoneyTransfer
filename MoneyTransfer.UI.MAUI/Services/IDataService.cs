using MoneyTransfer.UI.MAUI.Services.Models;

namespace MoneyTransfer.UI.MAUI.Services
{
    public interface IDataService
    {
        Task ApproveTransferRequestAsync(int transferId);
        Task<AccountDetails> GetAccountDetailsForUserAsync(string username);
        Task<List<TransferDetails>> GetCompletedTransfersForUserAsync(string username);
        Task<List<TransferDetails>> GetPendingTransfersForUserAsync(string username);
        Task<TransferDetails> GetTransferDetailsAsync(int transferId);
        Task RejectTransferRequestAsync(int transferId);
        Task RequestTransferAsync(string userFromName, string userToName, decimal amount);
        Task SendTransferAsync(string userFromName, string userToName, decimal amount);
    }
}
