using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MoneyTransfer.UI.MAUI.Pages;
using MoneyTransfer.UI.MAUI.Services;
using MoneyTransfer.UI.MAUI.Services.Models;
using System.Collections.ObjectModel;

namespace MoneyTransfer.UI.MAUI.PageModels
{
    public partial class PendingTransfersPageModel(IDataService dataService) : ObservableObject
    {
        private readonly IDataService _dataService = dataService;
        
        [ObservableProperty]
        private ReadOnlyCollection<TransferDetails> _transferDetails = default!;

        [ObservableProperty]
        private TransferDetails _selectedTransfer = default!;

        [RelayCommand]
        private void PageAppearing() => LoadData(); 

        [RelayCommand]
        private async Task ChangeSelection()
        {
            if (SelectedTransfer is not null && SelectedTransfer.Id > 0)
            {
                await Shell.Current.Navigation.PushModalAsync(new TransferDetailsPage(SelectedTransfer.Id));
            }
        }

        private async void LoadData()
        {
            // TODO: The passed in id is hard coded here for testing
            int userId = 1;
            TransferDetails = await _dataService.GetPendingTransfersForUserAsync(userId) ?? new ReadOnlyCollection<TransferDetails>(new List<TransferDetails> { Helpers.TransferNotFound });
        }
    }
}
