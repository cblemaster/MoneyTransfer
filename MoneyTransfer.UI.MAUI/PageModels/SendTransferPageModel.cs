using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MoneyTransfer.Core.DTO;
using MoneyTransfer.Core.Services.Data;
using MoneyTransfer.Core.Services.User;
using System.Collections.ObjectModel;

namespace MoneyTransfer.UI.MAUI.PageModels;

public partial class SendTransferPageModel(IDataService dataService, IUserService userService) : ObservableObject
{
    private readonly IDataService _dataService = dataService;
    private readonly IUserService _userService = userService;

    [ObservableProperty]
    private ReadOnlyCollection<UserDTO?> _users = default!;

    [ObservableProperty]
    private UserDTO _selectedUser = default!;

    [ObservableProperty]
    private string _amount = string.Empty;

    [RelayCommand]
    private async Task PageAppearing()
    {
        await LoadData();
        Amount = string.Empty;
    }

    [RelayCommand]
    private async Task SendTransfer()
    {
        if (!CanSendTransfer) { return; }
        if (SelectedUser is null) { return; }
        if (!decimal.TryParse(Amount, out decimal amount) || amount <= 0)
        { return; }

        UserDTO UserFrom = await _userService.GetUserById(AuthenticatedUserService.GetUserId());
        UserDTO UserTo = await _userService.GetUserById(SelectedUser.Id);
        if (UserFrom.Id.Equals(UserTo.Id)) { return; }

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

    private async Task LoadData() => Users = (await _userService.GetUsersNotLoggedIn());
}
