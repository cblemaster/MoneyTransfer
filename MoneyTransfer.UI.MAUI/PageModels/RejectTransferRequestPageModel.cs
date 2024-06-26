﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MoneyTransfer.Core.DTO;
using MoneyTransfer.Core.Services.Data;
using MoneyTransfer.Core.Services.User;

namespace MoneyTransfer.UI.MAUI.PageModels;

public partial class RejectTransferRequestPageModel(IDataService dataService, IUserService userService) : ObservableObject
{
    private readonly IDataService _dataService = dataService;
    private readonly IUserService _userService = userService;

    [ObservableProperty]
    private TransferDetailsDTO _transferDetails = default!;

    [ObservableProperty]
    private int transferId;

    partial void OnTransferIdChanged(int value) => LoadData();

    [RelayCommand]
    private async Task Reject()
    {
        if (!CanReject) { return; }

        await _dataService.RejectTransferRequestAsync(TransferDetails.Id, TransferDetails);
        await Shell.Current.DisplayAlert("Success!", "Transfer rejected.", "OK");
        await Shell.Current.GoToAsync("///CompletedTransfers");
    }

    [RelayCommand]
    private async Task Cancel()
    {
        if (!CanCancel) { return; }
        await Shell.Current.Navigation.PopAsync();
    }

    [ObservableProperty]
    private bool canReject;

    [ObservableProperty]
    private bool canCancel = true;

    private async void LoadData()
    {
        if (TransferId > 0)
        {
            TransferDetails = await _dataService.GetTransferDetailsAsync(TransferId) ?? TransferDetailsDTO.TransferNotFound;
            UserDTO loggedInUser = await _userService.GetUserById(AuthenticatedUserService.GetUserId());

            CanReject = TransferDetails.TransferStatus.Equals("Pending") && TransferDetails.TransferType.Equals("Request") && TransferDetails.UserFromName.Equals(loggedInUser.Username);
        }
    }
}
