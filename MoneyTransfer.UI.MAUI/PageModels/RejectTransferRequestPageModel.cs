using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MoneyTransfer.UI.MAUI.Services;
using MoneyTransfer.UI.MAUI.Services.Models;

namespace MoneyTransfer.UI.MAUI.PageModels
{
    public partial class RejectTransferRequestPageModel(IDataService dataService, IMockUserService userService) : ObservableObject
    {
        private readonly IDataService _dataService = dataService;
        private readonly IMockUserService _mockUserService = userService;

        [ObservableProperty]
        private TransferDetails _transferDetails = default!;

        [ObservableProperty]
        private int transferId;

        partial void OnTransferIdChanged(int value) => LoadData();

        [RelayCommand]
        private async Task Reject()
        {
            if (!CanReject) { return; }

            await _dataService.RejectTransferRequestAsync(TransferDetails.Id, TransferDetails);
            await Shell.Current.DisplayAlert("Success!", "Transfer rejected.", "OK");
            await Shell.Current.GoToAsync("///CompletedTransfers");
        }

        [RelayCommand]
        private async Task Cancel()
        {
            if (!CanCancel) { return; }
            await Shell.Current.Navigation.PopAsync();
        }

        [ObservableProperty]
        private bool canReject;

        [ObservableProperty]
        private bool canCancel = true;

        private async void LoadData()
        {
            if (TransferId > 0)
            {
                TransferDetails = await _dataService.GetTransferDetailsAsync(TransferId) ?? Helpers.TransferNotFound;
                User loggedInUser = (await _mockUserService.GetLoggedInUserAsync())!;

                CanReject = TransferDetails.TransferStatus == "Pending" && TransferDetails.TransferType == "Request" && TransferDetails.UserFromName == loggedInUser!.Username;
            }
        }
    }
}
