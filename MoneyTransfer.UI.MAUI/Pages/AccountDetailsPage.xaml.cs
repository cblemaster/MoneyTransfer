using MoneyTransfer.UI.MAUI.PageModels;

namespace MoneyTransfer.UI.MAUI.Pages;

public partial class AccountDetailsPage : ContentPage
{
    public AccountDetailsPage(AccountDetailsPageModel pageModel)
    {
        InitializeComponent();
        BindingContext = pageModel;
    }
}