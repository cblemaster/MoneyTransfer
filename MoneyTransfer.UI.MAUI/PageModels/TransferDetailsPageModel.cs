using CommunityToolkit.Mvvm.ComponentModel;
using MoneyTransfer.UI.MAUI.Services;
using MoneyTransfer.UI.MAUI.Services.Models;

namespace MoneyTransfer.UI.MAUI.PageModels
{
    public partial class TransferDetailsPageModel : ObservableObject
    {
        private readonly IDataService _dataService;

        public TransferDetailsPageModel(IDataService dataService)
        {
            _dataService = dataService;
            LoadData();
        }

        [ObservableProperty]
        TransferDetails _transferDetails = default!;

        private async void LoadData()
        {
            // TODO: The passed in id is hard coded here for testing
            TransferDetails = await _dataService.GetTransferDetailsAsync(1) ?? Helpers.TransferNotFound;
        }
    }
}
