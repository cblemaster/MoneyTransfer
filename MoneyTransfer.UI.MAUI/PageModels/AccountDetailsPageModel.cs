using CommunityToolkit.Mvvm.ComponentModel;
using MoneyTransfer.UI.MAUI.Services;
using MoneyTransfer.UI.MAUI.Services.Models;

namespace MoneyTransfer.UI.MAUI.PageModels
{
    public partial class AccountDetailsPageModel : ObservableObject
    {
        private readonly IDataService _dataService;

        public AccountDetailsPageModel(IDataService dataService)
        {
            _dataService = dataService;
            LoadData();
        }

        [ObservableProperty]
        AccountDetails _accountDetails = default!;

        private async void LoadData()
        {
            // TODO: The passed in id is hard coded here for testing
            AccountDetails = await _dataService.GetAccountDetailsForUserAsync(1) ?? Helpers.AccountNotFound;
        }
    }
}
