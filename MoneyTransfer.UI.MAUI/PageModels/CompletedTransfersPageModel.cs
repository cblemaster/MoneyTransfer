using CommunityToolkit.Mvvm.ComponentModel;
using MoneyTransfer.UI.MAUI.Services;
using MoneyTransfer.UI.MAUI.Services.Models;
using System.Collections.ObjectModel;

namespace MoneyTransfer.UI.MAUI.PageModels
{
    public partial class CompletedTransfersPageModel : ObservableObject
    {
        private readonly IDataService _dataService;

        public CompletedTransfersPageModel(IDataService dataService)
        {
            _dataService = dataService;
            LoadData();
        }

        [ObservableProperty]
        private ReadOnlyCollection<TransferDetails> _transferDetails = default!;

        [ObservableProperty]
        private TransferDetails _selectedTransfer = default!;

        private async void LoadData()
        {
            // TODO: The passed in id is hard coded here for testing
            TransferDetails = await _dataService.GetCompletedTransfersForUserAsync(1) ?? new ReadOnlyCollection<TransferDetails>(new List<TransferDetails> { Helpers.TransferNotFound });
        }
    }
}
