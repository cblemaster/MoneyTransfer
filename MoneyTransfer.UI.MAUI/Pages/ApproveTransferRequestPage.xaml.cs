using MoneyTransfer.UI.MAUI.PageModels;

namespace MoneyTransfer.UI.MAUI.Pages;

public partial class ApproveTransferRequestPage : ContentPage
{
    public ApproveTransferRequestPage(int id)
    {
        InitializeComponent();

        if (Shell.Current.Handler is not null && Shell.Current.Handler.MauiContext is not null)
        {
            ApproveTransferRequestPageModel? pageModel = Shell.Current.Handler.MauiContext.Services.GetService<ApproveTransferRequestPageModel>();
            if (pageModel != null)
            {
                BindingContext = pageModel;
                pageModel.TransferId = id;
            }
        }
    }
}