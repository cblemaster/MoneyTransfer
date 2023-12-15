using MoneyTransfer.UI.MAUI.PageModels;

namespace MoneyTransfer.UI.MAUI.Pages;

public partial class RequestTransferPage : ContentPage
{
	public RequestTransferPage(RequestTransferPageModel pageModel)
	{
		InitializeComponent();
        BindingContext = pageModel;
	}
}