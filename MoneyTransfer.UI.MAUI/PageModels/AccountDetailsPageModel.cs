using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MoneyTransfer.UI.MAUI.Services;
using MoneyTransfer.UI.MAUI.Services.Data;
using MoneyTransfer.UI.MAUI.Services.User;

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
            int currentUserId = AuthenticatedUserService.GetUserId();
            if (currentUserId > 0)
            {
                AccountDetails = await _dataService.GetAccountDetailsForUserAsync(currentUserId) ?? Helpers.AccountNotFound;
            }
        }
    }
}
