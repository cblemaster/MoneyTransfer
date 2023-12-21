using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
        private ReadOnlyCollection<TransferDetails> _transferDetails = default!;

        [ObservableProperty]
        private TransferDetails _selectedTransfer = default!;

        [RelayCommand]
        private void PageAppearing() => SelectedTransfer = null!;

        [RelayCommand]
        private async Task ChangeSelection()
        {
            if (SelectedTransfer is not null && SelectedTransfer.Id > 0)
            {
                await Shell.Current.GoToAsync(($"TransferDetails?id={SelectedTransfer.Id}"));
            }
        }

        private async void LoadData()
        {
            // TODO: The passed in id is hard coded here for testing
            TransferDetails = await _dataService.GetPendingTransfersForUserAsync(1) ?? new ReadOnlyCollection<TransferDetails>(new List<TransferDetails> { Helpers.TransferNotFound });
            SelectedTransfer = null!;
        }
    }
}
