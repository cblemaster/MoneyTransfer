using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MoneyTransfer.Core.DTO;
using MoneyTransfer.Core.Services.Data;
using MoneyTransfer.Core.Services.User;
using MoneyTransfer.UI.MAUI.Pages;
using System.Collections.ObjectModel;

namespace MoneyTransfer.UI.MAUI.PageModels
{
    public partial class CompletedTransfersPageModel(IDataService dataService) : ObservableObject
    {
        private readonly IDataService _dataService = dataService;

        [ObservableProperty]
        private ReadOnlyCollection<TransferDetailsDTO?> _transferDetails = default!;

        [ObservableProperty]
        private TransferDetailsDTO _selectedTransfer = default!;

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

        private async void LoadData() => TransferDetails =
            await _dataService.GetCompletedTransfersForUserAsync(AuthenticatedUserService.GetUserId());
    }
}
