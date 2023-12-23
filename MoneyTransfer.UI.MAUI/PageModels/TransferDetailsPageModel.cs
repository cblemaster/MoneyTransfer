using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MoneyTransfer.UI.MAUI.Pages;
using MoneyTransfer.UI.MAUI.Services;
using MoneyTransfer.UI.MAUI.Services.Models;

namespace MoneyTransfer.UI.MAUI.PageModels
{
    public partial class TransferDetailsPageModel(IDataService dataService) : ObservableObject
    {
        private readonly IDataService _dataService = dataService;
        
        [ObservableProperty]
        private TransferDetails _transferDetails = default!;

        [ObservableProperty]
        private int transferId;

        partial void OnTransferIdChanged(int value) => LoadData();

        [RelayCommand]
        private async Task Approve()
        {
            if (!CanApprove) { return; }
            if (TransferDetails.Id <= 0) { return; }
            await Shell.Current.Navigation.PushModalAsync(new ApproveTransferRequestPage(TransferDetails.Id));
        }

        [RelayCommand]
        private async Task Reject()
        {
            if (!CanReject) { return; }
            if (TransferDetails.Id <= 0) { return; }
            await Shell.Current.Navigation.PushModalAsync(new RejectTransferRequestPage(TransferDetails.Id));
        }

        [RelayCommand]
        private async Task Cancel()
        {
            if (!CanCancel) { return; }
            await Shell.Current.Navigation.PopAsync();
        }

        [ObservableProperty]
        private bool canApprove = true;  // TODO: if type == request && status == pending && user from is the current logged in user

        [ObservableProperty]
        private bool canReject = true; // TODO: if type == request && status == pending && user from is the current logged in user

        [ObservableProperty]
        private bool canCancel = true;

        private async void LoadData()
        {
            if (TransferId > 0)
            {
                TransferDetails = await _dataService.GetTransferDetailsAsync(TransferId) ?? Helpers.TransferNotFound;
                CanApprove = TransferDetails.TransferStatus == "Pending";
                CanReject = CanApprove;
            }
        }
    }
}
