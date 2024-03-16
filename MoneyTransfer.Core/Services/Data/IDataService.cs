using MoneyTransfer.Core.DTO;
using System.Collections.ObjectModel;

namespace MoneyTransfer.UI.MAUI.Services.Data
{
    public interface IDataService
    {
        Task ApproveTransferRequestAsync(int transferId, TransferDetailsDTO transfer);
        Task<AccountDetailsDTO> GetAccountDetailsForUserAsync(int userId);
        Task<ReadOnlyCollection<TransferDetailsDTO>> GetCompletedTransfersForUserAsync(int userId);
        Task<ReadOnlyCollection<TransferDetailsDTO>> GetPendingTransfersForUserAsync(int userId);
        Task<TransferDetailsDTO> GetTransferDetailsAsync(int transferId);
        Task RejectTransferRequestAsync(int transferId, TransferDetailsDTO transfer);
        Task RequestTransferAsync(string userFromName, string userToName, decimal amount);
        Task SendTransferAsync(string userFromName, string userToName, decimal amount);
    }
}
