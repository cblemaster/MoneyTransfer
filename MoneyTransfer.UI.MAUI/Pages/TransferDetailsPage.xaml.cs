using MoneyTransfer.UI.MAUI.PageModels;

namespace MoneyTransfer.UI.MAUI.Pages;

public partial class TransferDetailsPage : ContentPage
{
    public TransferDetailsPage(int id)
    {
        InitializeComponent();

        if (Handler is not null && Handler.MauiContext is not null)
        {
            TransferDetailsPageModel? pageModel = Handler.MauiContext.Services.GetService<TransferDetailsPageModel>();
            if (pageModel != null)
            {
                BindingContext = pageModel;
                pageModel.TransferId = id;
            }
        }
    }
}