using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MoneyTransfer.Core.DTO;
using MoneyTransfer.Core.Services.Data;
using MoneyTransfer.Core.Services.User;

namespace MoneyTransfer.UI.MAUI.PageModels
{
    public partial class ApproveTransferRequestPageModel(IDataService dataService, IUserService userService) : ObservableObject
    {
        private readonly IDataService _dataService = dataService;
        private readonly IUserService _userService = userService;

        [ObservableProperty]
        private TransferDetailsDTO _transferDetails = default!;

        [ObservableProperty]
        private int transferId;

        partial void OnTransferIdChanged(int value) => LoadData();

        [RelayCommand]
        private async Task Approve()
        {
            if (!CanApprove) { return; }

            UserDTO loggedInUser = await _userService.GetUserById(AuthenticatedUserService.GetUserId());
            decimal currentBalance = (await _dataService.GetAccountDetailsForUserAsync(loggedInUser.Id)).CurrentBalance;
            if (currentBalance <= 0 || currentBalance < TransferDetails.Amount)
            {
                await Shell.Current.DisplayAlert("Error!", "Only transfer requests with an amount less than or equal to your current balance can be approved.", "OK");
                return;
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
                TransferDetails = await _dataService.GetTransferDetailsAsync(TransferId) ?? TransferDetailsDTO.TransferNotFound;
                UserDTO loggedInUser = await _userService.GetUserById(AuthenticatedUserService.GetUserId());

                CanApprove = TransferDetails.TransferStatus == "Pending" && TransferDetails.TransferType == "Request" && TransferDetails.UserFromName == loggedInUser.Username;
            }
        }
    }
}
