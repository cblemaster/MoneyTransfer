using MoneyTransfer.UI.MAUI.PageModels;

namespace MoneyTransfer.UI.MAUI.Pages;

public partial class RejectTransferRequestPage : ContentPage
{
	public RejectTransferRequestPage(RejectTransferRequestPageModel pageModel)
	{
		InitializeComponent();
        BindingContext = pageModel;
	}
}