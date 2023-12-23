using MoneyTransfer.UI.MAUI.PageModels;

namespace MoneyTransfer.UI.MAUI.Pages;

public partial class RejectTransferRequestPage : ContentPage
{
    public RejectTransferRequestPage(int id)
    {
        InitializeComponent();

        if (Shell.Current.Handler is not null && Shell.Current.Handler.MauiContext is not null)
        {
            RejectTransferRequestPageModel? pageModel = Shell.Current.Handler.MauiContext.Services.GetService<RejectTransferRequestPageModel>();
            if (pageModel != null)
            {
                BindingContext = pageModel;
                pageModel.TransferId = id;
            }
        }
    }
}