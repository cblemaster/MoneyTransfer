using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MoneyTransfer.UI.MAUI.Pages;
using MoneyTransfer.UI.MAUI.Services;
using MoneyTransfer.UI.MAUI.Services.Data;
using MoneyTransfer.UI.MAUI.Services.User;
using System.Collections.ObjectModel;

namespace MoneyTransfer.UI.MAUI.PageModels
{
    public partial class PendingTransfersPageModel(IDataService dataService, IUserService userService) : ObservableObject
    {
        private readonly IDataService _dataService = dataService;
        private readonly IUserService _userService = userService;

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
            UserDTO loggedInUser = await _userService.GetUserById(_userService.GetUserId());
            TransferDetails = await _dataService.GetPendingTransfersForUserAsync(loggedInUser.Id) ?? new ReadOnlyCollection<TransferDetails>(new List<TransferDetails> { Helpers.TransferNotFound });
        }
    }
}
