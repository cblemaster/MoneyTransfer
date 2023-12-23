using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MoneyTransfer.UI.MAUI.Services;
using MoneyTransfer.UI.MAUI.Services.Models;

namespace MoneyTransfer.UI.MAUI.PageModels
{
    public partial class ApproveTransferRequestPageModel(IDataService dataService, IMockUserService userService) : ObservableObject
    {
        private readonly IDataService _dataService = dataService;
        private readonly IMockUserService _mockUserService = userService;

        [ObservableProperty]
        private TransferDetails _transferDetails = default!;

        [ObservableProperty]
        private int transferId;

        partial void OnTransferIdChanged(int value) => LoadData();

        [RelayCommand]
        private async Task Approve()
        {
            if (!CanApprove) { return; }

            User loggedInUser = (await _mockUserService.GetLoggedInUserAsync())!;
            decimal currentBalance = (await _dataService.GetAccountDetailsForUserAsync(loggedInUser.Id)).CurrentBalance;
            if (currentBalance <= 0 || currentBalance < TransferDetails.Amount)
            {
                await Shell.Current.DisplayAlert("Error!", "Only transfer requests with an amount less than or equal to your current balance can be approved.", "OK");
            }

            await _dataService.ApproveTransferRequestAsync(TransferDetails.Id, TransferDetails);
            await Shell.Current.DisplayAlert("Success!", "Transfer approved.", "OK");
            await Shell.Current.GoToAsync("///CompletedTransfers");
        }

        [RelayCommand]
        private async Task Cancel()
        {
            if (!CanCancel) { return; }
            await Shell.Current.Navigation.PopAsync();
        }

        [ObservableProperty]
        private bool canApprove;

        [ObservableProperty]
        private bool canCancel = true;

        private async void LoadData()
        {
            if (TransferId > 0)
            {
                TransferDetails = await _dataService.GetTransferDetailsAsync(TransferId) ?? Helpers.TransferNotFound;
                User loggedInUser = (await _mockUserService.GetLoggedInUserAsync())!;

                CanApprove = TransferDetails.TransferStatus == "Pending" && TransferDetails.TransferType == "Request" && TransferDetails.UserFromName == loggedInUser!.Username;
            }
        }
    }
}
