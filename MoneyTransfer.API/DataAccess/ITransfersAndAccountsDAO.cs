using MoneyTransfer.API.Entities;

namespace MoneyTransfer.API.DataAccess
{
    public interface ITransfersAndAccountsDAO
    {
        void ApproveTransferRequest();
        Task<Account> GetAccountDetailsForUserAsync(string username);
        Task<List<Transfer>> GetCompletedTransfersForUserAsync(string username);
        Task<List<Transfer>> GetPendingTransfersForUserAsync(string username);
        Task<Transfer> GetTransferDetailsAsync(int transferId);
        void RejectTransferRequest();
        void RequestTransfer();
        void SendTransfer();
    }
}
