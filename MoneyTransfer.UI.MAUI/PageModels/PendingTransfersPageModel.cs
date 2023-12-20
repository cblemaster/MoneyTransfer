using CommunityToolkit.Mvvm.ComponentModel;
using MoneyTransfer.UI.MAUI.Services;
using MoneyTransfer.UI.MAUI.Services.Models;
using System.Collections.ObjectModel;

namespace MoneyTransfer.UI.MAUI.PageModels
{
    public partial class PendingTransfersPageModel : ObservableObject
    {
        private readonly IDataService _dataService;

        public PendingTransfersPageModel(IDataService dataService)
        {
            _dataService = dataService;
            LoadData();
        }

        [ObservableProperty]
        ReadOnlyCollection<TransferDetails> _transferDetails = default!;

        [ObservableProperty]
        TransferDetails _selectedTransfer = default!;

        private async void LoadData()
        {
            // TODO: The passed in id is hard coded here for testing
            TransferDetails = await _dataService.GetPendingTransfersForUserAsync(1) ?? new ReadOnlyCollection<TransferDetails>(new List<TransferDetails> { Helpers.TransferNotFound });
        }
    }
}
