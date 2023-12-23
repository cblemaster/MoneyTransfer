using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MoneyTransfer.UI.MAUI.Services;
using MoneyTransfer.UI.MAUI.Services.Models;

namespace MoneyTransfer.UI.MAUI.PageModels
{
    public partial class AccountDetailsPageModel(IDataService dataService) : ObservableObject
    {
        private readonly IDataService _dataService = dataService;
        
        [ObservableProperty]
        private AccountDetails _accountDetails = default!;

        [RelayCommand]
        private void PageAppearing() => LoadData();

        private async void LoadData()
        {
            // TODO: The passed in userid is hard coded here
            // We will want to get it by accessing the current logged in user
            // once a user service is available...
            int userId = 1;
            AccountDetails = await _dataService.GetAccountDetailsForUserAsync(userId) ?? Helpers.AccountNotFound;
        }
    }
}
