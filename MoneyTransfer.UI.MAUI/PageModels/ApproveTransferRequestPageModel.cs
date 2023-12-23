using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MoneyTransfer.UI.MAUI.Pages;
using MoneyTransfer.UI.MAUI.Services;
using MoneyTransfer.UI.MAUI.Services.Models;

namespace MoneyTransfer.UI.MAUI.PageModels
{
    public partial class ApproveTransferRequestPageModel(IDataService dataService) : ObservableObject
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
            if (TransferDetails.TransferStatus != "Pending")
            {
                await Shell.Current.DisplayAlert("Error!", "Only transfer requests with a status of Pending can be approved.", "OK");
            }
            
            int currentUserId = 2; // TODO: Needs to be the logged in user's id
            decimal currentBalance = (await _dataService.GetAccountDetailsForUserAsync(currentUserId)).CurrentBalance;
            if (currentBalance <= 0 || currentBalance < TransferDetails.Amount)
            {
                await Shell.Current.DisplayAlert("Error!", "Only transfer requests with an amount less than or equal to your current balance can be approved.", "OK");
            }
            
            await _dataService.ApproveTransferRequestAsync(TransferDetails.Id, TransferDetails);
            await Shell.Current.DisplayAlert("Success!", "Transfer approved.", "OK");
            await Shell.Current.GoToAsync("CompletedTransfers");
        }

        [RelayCommand]
        private async Task Cancel()
        {
            if (!CanCancel) { return; }
            await Shell.Current.Navigation.PopAsync();
        }

        [ObservableProperty]
        private bool canApprove = true; // TODO: if type == request && status == pending && user from is the current logged in user

        [ObservableProperty]
        private bool canCancel = true;

        private async void LoadData()
        {
            if (TransferId > 0)
            {
                TransferDetails = await _dataService.GetTransferDetailsAsync(TransferId) ?? Helpers.TransferNotFound;
                CanApprove = TransferDetails.TransferStatus == "Pending";
            }
        }
    }
}
