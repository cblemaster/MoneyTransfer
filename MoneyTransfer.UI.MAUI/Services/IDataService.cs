using MoneyTransfer.UI.MAUI.Services.Models;
using System.Collections.ObjectModel;

namespace MoneyTransfer.UI.MAUI.Services
{
    public interface IDataService
    {
        Task ApproveTransferRequestAsync(int transferId, TransferDetails transfer);
        Task<AccountDetails> GetAccountDetailsForUserAsync(int userId);
        Task<ReadOnlyCollection<TransferDetails>> GetCompletedTransfersForUserAsync(int userId);
        Task<ReadOnlyCollection<TransferDetails>> GetPendingTransfersForUserAsync(int userId);
        Task<TransferDetails> GetTransferDetailsAsync(int transferId);
        Task RejectTransferRequestAsync(int transferId, TransferDetails transfer);
        Task RequestTransferAsync(string userFromName, string userToName, decimal amount);
        Task SendTransferAsync(string userFromName, string userToName, decimal amount);
    }
}
