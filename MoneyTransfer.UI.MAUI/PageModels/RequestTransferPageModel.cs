using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MoneyTransfer.UI.MAUI.Services;
using System.Collections.ObjectModel;

namespace MoneyTransfer.UI.MAUI.PageModels
{
    public partial class RequestTransferPageModel(IDataService dataService, IMockUserService mockUserService) : ObservableObject
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
        private async Task RequestTransfer()
        {
            if (!CanRequestTransfer) { return; }
            if (SelectedUser is null)
            {
                CanRequestTransfer = false;
                return;
            }
            if (!decimal.TryParse(Amount, out decimal amount) || amount <= 0)
            {
                CanRequestTransfer = false;
                return;
            }

            User UserTo = (await _mockUserService.GetLoggedInUserAsync())!;
            User UserFrom = (await _mockUserService.GetUserById(SelectedUser.Id))!;

            if (UserFrom.Id == UserTo.Id)
            {
                CanRequestTransfer = false;
                return;
            }

            await _dataService.RequestTransferAsync(UserFrom.Username, UserTo.Username, amount);
            await Shell.Current.DisplayAlert("Success!", "Request submitted, go to Pending Transfers to see it.", "OK");
        }

        [ObservableProperty]
        private bool _canRequestTransfer = true;

        private async void LoadData()
        {
            User loggedInUser = (await _mockUserService.GetLoggedInUserAsync())!;
            Users = (await _mockUserService.AllUsersNotLoggedIn(loggedInUser))!;
        }
    }
}
