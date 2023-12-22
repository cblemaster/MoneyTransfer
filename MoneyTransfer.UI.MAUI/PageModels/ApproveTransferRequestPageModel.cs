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
        private void Approve()
        {
            if (!CanApprove) { return; }
            // TODO: call logic to approve the transfer, nav nack to transfer details,
            // passing the transfer's id (*make sure that page shows the new status!*)
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
            // TODO: The passed in id is hard coded here for testing
            TransferDetails = await _dataService.GetTransferDetailsAsync(TransferId) ?? Helpers.TransferNotFound;
        }
    }
}
