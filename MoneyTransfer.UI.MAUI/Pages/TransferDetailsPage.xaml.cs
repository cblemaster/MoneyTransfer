using MoneyTransfer.UI.MAUI.PageModels;

namespace MoneyTransfer.UI.MAUI.Pages;

public partial class TransferDetailsPage : ContentPage
{
    public TransferDetailsPage(TransferDetailsPageModel pageModel)
    {
        InitializeComponent();
        BindingContext = pageModel;
    }
}