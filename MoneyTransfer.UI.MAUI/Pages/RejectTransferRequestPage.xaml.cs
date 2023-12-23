using MoneyTransfer.UI.MAUI.PageModels;

namespace MoneyTransfer.UI.MAUI.Pages;

public partial class RejectTransferRequestPage : ContentPage
{
    public RejectTransferRequestPage(int id)
    {
        InitializeComponent();

        if (Handler is not null && Handler.MauiContext is not null)
        {
            RejectTransferRequestPageModel? pageModel = Handler.MauiContext.Services.GetService<RejectTransferRequestPageModel>();
            if (pageModel != null)
            {
                BindingContext = pageModel;
                pageModel.TransferId = id;
            }
        }
    }
}