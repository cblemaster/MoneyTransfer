using MoneyTransfer.UI.MAUI.PageModels;

namespace MoneyTransfer.UI.MAUI.Pages;

public partial class SendTransferPage : ContentPage
{
	public SendTransferPage(SendTransferPageModel pageModel)
	{
		InitializeComponent();
        BindingContext = pageModel;
	}
}