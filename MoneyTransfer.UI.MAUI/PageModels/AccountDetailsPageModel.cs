using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MoneyTransfer.Core.DTO;
using MoneyTransfer.Core.Services.Data;
using MoneyTransfer.Core.Services.User;

namespace MoneyTransfer.UI.MAUI.PageModels
{
    public partial class AccountDetailsPageModel(IDataService dataService) : ObservableObject
    {
        private readonly IDataService _dataService = dataService;

        [ObservableProperty]
        private AccountDetailsDTO _accountDetails = default!;

        [RelayCommand]
        private void PageAppearing() => LoadData();

        private async void LoadData() => AccountDetails = await _dataService.GetAccountDetailsForUserAsync(AuthenticatedUserService.GetUserId()) ?? AccountDetailsDTO.AccountNotFound;
    }
}
