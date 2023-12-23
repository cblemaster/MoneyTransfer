using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MoneyTransfer.UI.MAUI.Pages;
using MoneyTransfer.UI.MAUI.Services;
using MoneyTransfer.UI.MAUI.Services.Models;

namespace MoneyTransfer.UI.MAUI.PageModels
{
    public partial class RejectTransferRequestPageModel(IDataService dataService) : ObservableObject
    {
        private readonly IDataService _dataService = dataService;
        
        [ObservableProperty]
        private TransferDetails _transferDetails = default!;

        [ObservableProperty]
        private int transferId;

        partial void OnTransferIdChanged(int value) => LoadData();

        [RelayCommand]
        private async Task Reject()
        {
            if (!CanReject) { return; }
            if (TransferDetails.TransferStatus != "Pending")
            {
                await Shell.Current.DisplayAlert("Error!", "Only transfer requests with a status of Pending can be rejected.", "OK");
            }

            await _dataService.RejectTransferRequestAsync(TransferDetails.Id, TransferDetails);
            await Shell.Current.DisplayAlert("Success!", "Transfer rejected.", "OK");
            await Shell.Current.GoToAsync("CompletedTransfers");
        }

        [RelayCommand]
        private async Task Cancel()
        {
            if (!CanCancel) { return; }
            await Shell.Current.Navigation.PopAsync();
        }

        [ObservableProperty]
        private bool canReject = true; // TODO: if type == request && status == pending && user from is the current logged in user

        [ObservableProperty]
        private bool canCancel = true;

        private async void LoadData()
        {
            if (TransferId > 0)
            {
                TransferDetails = await _dataService.GetTransferDetailsAsync(TransferId) ?? Helpers.TransferNotFound;
                CanReject = TransferDetails.TransferStatus == "Pending";
            }
        }
    }
}
