using MoneyTransfer.UI.MAUI.PageModels;

namespace MoneyTransfer.UI.MAUI.Pages;

public partial class TransferDetailsPage : ContentPage
{
    public TransferDetailsPage(int id)
    {
        InitializeComponent();

        if (Shell.Current.Handler is not null && Shell.Current.Handler.MauiContext is not null)
        {
            TransferDetailsPageModel? pageModel = Shell.Current.Handler.MauiContext.Services.GetService<TransferDetailsPageModel>();
            if (pageModel is not null)
            {
                BindingContext = pageModel;
                pageModel.TransferId = id;
            }
        }
    }
}