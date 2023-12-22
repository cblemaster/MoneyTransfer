using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MoneyTransfer.UI.MAUI.Services;
using MoneyTransfer.UI.MAUI.Services.Models;

namespace MoneyTransfer.UI.MAUI.PageModels
{
    [QueryProperty(nameof(TransferId), "id")]
    public partial class TransferDetailsPageModel : ObservableObject
    {
        private readonly IDataService _dataService;

        public TransferDetailsPageModel(IDataService dataService) => _dataService = dataService;

        [ObservableProperty]
        private TransferDetails _transferDetails = default!;

        [ObservableProperty]
        private int transferId;

        partial void OnTransferIdChanged(int value) => LoadData();

        [RelayCommand]
        private async Task Approve()
        {
            if (!CanApprove) { return; }
            if (TransferId <= 0) { return; }
            await Shell.Current.GoToAsync(($"ApproveTransfer?id={TransferId}"));
        }

        [RelayCommand]
        private async Task Reject()
        {
            if (!CanReject) { return; }
            if (TransferId <= 0) { return; }
            await Shell.Current.GoToAsync(($"RejectTransfer?id={TransferId}"));
        }

        [RelayCommand]
        private async Task Cancel()
        {
            if (!CanCancel) { return; }
            await Shell.Current.GoToAsync("..");
        }

        [ObservableProperty]
        private bool canApprove = true;  // TODO: if type == request && status == pending && user from is the current logged in user

        [ObservableProperty]
        private bool canReject = true; // TODO: if type == request && status == pending && user from is the current logged in user

        [ObservableProperty]
        private bool canCancel = true;

        private async void LoadData()
        {
            TransferDetails = await _dataService.GetTransferDetailsAsync(TransferId) ?? Helpers.TransferNotFound;
        }
    }
}
