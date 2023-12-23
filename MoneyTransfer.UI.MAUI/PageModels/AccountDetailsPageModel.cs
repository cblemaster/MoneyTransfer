using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MoneyTransfer.UI.MAUI.Services;
using MoneyTransfer.UI.MAUI.Services.Models;

namespace MoneyTransfer.UI.MAUI.PageModels
{
    public partial class AccountDetailsPageModel(IDataService dataService, IMockUserService userService) : ObservableObject
    {
        private readonly IDataService _dataService = dataService;
        private readonly IMockUserService _mockUserService = userService;

        [ObservableProperty]
        private AccountDetails _accountDetails = default!;

        [RelayCommand]
        private void PageAppearing() => LoadData();

        private async void LoadData()
        {
            User loggedInUser = (await _mockUserService.GetLoggedInUserAsync())!;
            AccountDetails = await _dataService.GetAccountDetailsForUserAsync(loggedInUser.Id) ?? Helpers.AccountNotFound;
        }
    }
}
