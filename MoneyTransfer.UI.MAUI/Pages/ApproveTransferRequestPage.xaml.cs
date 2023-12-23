using MoneyTransfer.UI.MAUI.PageModels;

namespace MoneyTransfer.UI.MAUI.Pages;

public partial class ApproveTransferRequestPage : ContentPage
{
    public ApproveTransferRequestPage(int id)
    {
        InitializeComponent();

        if (Handler is not null && Handler.MauiContext is not null)
        {
            ApproveTransferRequestPageModel? pageModel = Handler.MauiContext.Services.GetService<ApproveTransferRequestPageModel>();
            if (pageModel != null)
            {
                BindingContext = pageModel;
                pageModel.TransferId = id;
            }
        }
    }
}