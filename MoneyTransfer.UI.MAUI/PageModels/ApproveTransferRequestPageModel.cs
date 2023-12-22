using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MoneyTransfer.UI.MAUI.Services;
using MoneyTransfer.UI.MAUI.Services.Models;

namespace MoneyTransfer.UI.MAUI.PageModels
{
    [QueryProperty(nameof(TransferId), "id")]
    public partial class ApproveTransferRequestPageModel : ObservableObject
    {
        private readonly IDataService _dataService;

        public ApproveTransferRequestPageModel(IDataService dataService) => _dataService = dataService;

        [ObservableProperty]
        private TransferDetails _transferDetails = default!;

        [ObservableProperty]
        private int transferId;

        partial void OnTransferIdChanged(int value) => LoadData();

        [RelayCommand]
        private async Task Approve()
        {
            // TODO: Error handling...
            if (!CanApprove) { return; }
            if (TransferDetails.TransferStatus != "Pending") { return; }
            
            int currentUserId = 2; // TODO: Needs to be the logged in user's id
            decimal currentBalance = (await _dataService.GetAccountDetailsForUserAsync(currentUserId)).CurrentBalance;
            if (currentBalance <= 0 || currentBalance < TransferDetails.Amount) { return; }
            
            await _dataService.ApproveTransferRequestAsync(TransferDetails.Id, TransferDetails);
            await Shell.Current.GoToAsync(($"TransferDetails?id={TransferDetails.Id}"));
        }

        [RelayCommand]
        private async Task Cancel()
        {
            if (!CanCancel) { return; }
            await Shell.Current.GoToAsync("..");
        }

        [ObservableProperty]
        private bool canApprove = true; // TODO: if type == request && status == pending && user from is the current logged in user

        [ObservableProperty]
        private bool canCancel = true;

        private async void LoadData()
        {
            TransferDetails = await _dataService.GetTransferDetailsAsync(TransferId) ?? Helpers.TransferNotFound;
        }
    }
}
