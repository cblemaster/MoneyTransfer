using MoneyTransfer.UI.MAUI.PageModels;

namespace MoneyTransfer.UI.MAUI.Pages;

public partial class CompletedTransfersPage : ContentPage
{
    public CompletedTransfersPage(CompletedTransfersPageModel pageModel)
    {
        InitializeComponent();
        BindingContext = pageModel;
    }
}