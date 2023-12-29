using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MoneyTransfer.UI.MAUI.Services.Data;
using MoneyTransfer.UI.MAUI.Services.User;
using System.Collections.ObjectModel;

namespace MoneyTransfer.UI.MAUI.PageModels
{
    public partial class RequestTransferPageModel(IDataService dataService, IUserService userService) : ObservableObject
    {
        private readonly IDataService _dataService = dataService;
        private readonly IUserService _userService = userService;

        [ObservableProperty]
        private ReadOnlyCollection<User> _users = default!;

        [ObservableProperty]
        private User _selectedUser = default!;

        [ObservableProperty]
        private string _amount = default!;

       [RelayCommand]
        private async Task PageAppearing()
        {
            await LoadData();
            Amount = string.Empty;
        }

        [RelayCommand]
        private async Task RequestTransfer()
        {
            if (!CanRequestTransfer) { return; }
            if (SelectedUser is null) { return; }
            if (!decimal.TryParse(Amount, out decimal amount) || amount <= 0)
            { return; }

            UserDTO UserTo = await _userService.GetUserById(AuthenticatedUserService.GetUserId());
            UserDTO UserFrom = await _userService.GetUserById(SelectedUser.Id);

            if (UserFrom.Id == UserTo.Id) { return; }

            await _dataService.RequestTransferAsync(UserFrom.Username, UserTo.Username, amount);
            await Shell.Current.DisplayAlert("Success!", "Request submitted, go to Pending Transfers to see it.", "OK");
        }

        [ObservableProperty]
        private bool _canRequestTransfer = true;

        private async Task LoadData() => Users = (await _userService.GetUsersNotLoggedIn())!;
    }
}
