using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MoneyTransfer.UI.MAUI.Services;
using MoneyTransfer.UI.MAUI.Services.Models;

namespace MoneyTransfer.UI.MAUI.PageModels
{
    public partial class TransferDetailsPageModel : ObservableObject
    {
        private readonly IDataService _dataService;

        public TransferDetailsPageModel(IDataService dataService)
        {
            _dataService = dataService;
            LoadData();
        }

        [ObservableProperty]
        private TransferDetails _transferDetails = default!;

        [RelayCommand]
        private void Approve()
        {
            if (!CanApprove) { return; }
            // TODO: nav to approve transfer request page, passing the transfer's id
        }

        [RelayCommand]
        private void Reject()
        {
            if (!CanReject) { return; }
            // TODO: nav to reject transfer request page, passing the transfer's id
        }

        [RelayCommand]
        private void Cancel()
        {
            if (!CanCancel) { return; }
            // TODO: nav back to transfer list, either go back one page in the nav stack,
            //      or nav to pending transfers page if transfer status is pending,
            //      otherwise nav to completed transfers page
        }

        [ObservableProperty]
        private bool canApprove = true;  // TODO: if type == request && status == pending && user from is the current logged in user

        [ObservableProperty]
        private bool canReject = true; // TODO: if type == request && status == pending && user from is the current logged in user

        [ObservableProperty]
        private bool canCancel = true;

        private async void LoadData()
        {
            // TODO: The passed in id is hard coded here for testing
            TransferDetails = await _dataService.GetTransferDetailsAsync(1) ?? Helpers.TransferNotFound;
        }
    }
}
