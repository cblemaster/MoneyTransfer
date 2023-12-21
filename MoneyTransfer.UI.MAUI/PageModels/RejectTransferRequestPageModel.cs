using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MoneyTransfer.UI.MAUI.Services;
using MoneyTransfer.UI.MAUI.Services.Models;

namespace MoneyTransfer.UI.MAUI.PageModels
{
    public partial class RejectTransferRequestPageModel : ObservableObject
    {
        private readonly IDataService _dataService;

        public RejectTransferRequestPageModel(IDataService dataService)
        {
            _dataService = dataService;
            LoadData();
        }

        [ObservableProperty]
        private TransferDetails _transferDetails = default!;

        [RelayCommand]
        private void Reject()
        {
            if (!CanReject) { return; }
            // TODO: call logic to reject the transfer, nav nack to transfer details,
            // passing the transfer's id (*make sure that page shows the new status!*)
        }

        [RelayCommand]
        private void Cancel()
        {
            if (!CanCancel) { return; }
            // TODO: nav nack to transfer details, passing the transfer's id
        }

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
