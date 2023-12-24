using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MoneyTransfer.UI.MAUI.Services;
using System.Collections.ObjectModel;

namespace MoneyTransfer.UI.MAUI.PageModels
{
    public partial class SendTransferPageModel(IDataService dataService, IMockUserService mockUserService) : ObservableObject
    {
        private readonly IDataService _dataService = dataService;
        private readonly IMockUserService _mockUserService = mockUserService;

        [ObservableProperty]
        private ReadOnlyCollection<User> _users = default!;

        [ObservableProperty]
        private User _selectedUser = default!;

        [ObservableProperty]
        private string _amount = default!;

        [RelayCommand]
        private void PageAppearing() => LoadData();

        [RelayCommand]
        private async Task SendTransfer()
        {
            if (!CanSendTransfer) { return; }
            if (SelectedUser is null) { return; }
            if (!decimal.TryParse(Amount, out decimal amount) || amount <= 0)
                { return; }

            User UserFrom = (await _mockUserService.GetLoggedInUserAsync())!;
            User UserTo = (await _mockUserService.GetUserById(SelectedUser.Id))!;
            if (UserFrom.Id == UserTo.Id) { return; }

            decimal userFromBalance = (await _dataService.GetAccountDetailsForUserAsync(UserFrom.Id)).CurrentBalance;
            if (userFromBalance < amount)
            {
                await Shell.Current.DisplayAlert("Error!", "Only transfer requests with an amount less than or equal to your current balance can be sent.", "OK");
                return;
            }

            await _dataService.SendTransferAsync(UserFrom.Username, UserTo.Username, amount);
            await Shell.Current.DisplayAlert("Success!", "Transfer sent, go to Completed Transfers to see it.", "OK");
        }

        [ObservableProperty]
        private bool _canSendTransfer = true;

        private async void LoadData()
        {
            User loggedInUser = (await _mockUserService.GetLoggedInUserAsync())!;
            Users = (await _mockUserService.AllUsersNotLoggedIn(loggedInUser))!;
        }
    }
}
