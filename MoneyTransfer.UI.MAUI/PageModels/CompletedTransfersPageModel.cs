﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MoneyTransfer.UI.MAUI.Pages;
using MoneyTransfer.UI.MAUI.Services.Data;
using MoneyTransfer.UI.MAUI.Services.User;
using System.Collections.ObjectModel;

namespace MoneyTransfer.UI.MAUI.PageModels
{
    public partial class CompletedTransfersPageModel(IDataService dataService) : ObservableObject
    {
        private readonly IDataService _dataService = dataService;

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

        private async void LoadData() => TransferDetails =
            await _dataService.GetCompletedTransfersForUserAsync(AuthenticatedUserService.GetUserId());
    }
}
