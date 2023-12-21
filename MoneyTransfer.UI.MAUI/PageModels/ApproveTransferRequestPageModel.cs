using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MoneyTransfer.UI.MAUI.Services;
using MoneyTransfer.UI.MAUI.Services.Models;

namespace MoneyTransfer.UI.MAUI.PageModels
{
    public partial class ApproveTransferRequestPageModel : ObservableObject
    {
        private readonly IDataService _dataService;

        public ApproveTransferRequestPageModel(IDataService dataService)
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
        }

        [RelayCommand]
        private void Cancel()
        {
            if (!CanCancel) { return; }
        }

        [ObservableProperty]
        private bool canApprove = true; // TODO: if type == request && status == pending && user from is the current logged in user

        [ObservableProperty]
        private bool canCancel = true;

        private async void LoadData()
        {
            // TODO: The passed in id is hard coded here for testing
            TransferDetails = await _dataService.GetTransferDetailsAsync(1) ?? Helpers.TransferNotFound;
        }
    }
}
