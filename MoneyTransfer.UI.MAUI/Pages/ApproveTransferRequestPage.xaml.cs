using MoneyTransfer.UI.MAUI.PageModels;

namespace MoneyTransfer.UI.MAUI.Pages;

public partial class ApproveTransferRequestPage : ContentPage
{
	public ApproveTransferRequestPage(ApproveTransferRequestPageModel pageModel)
	{
		InitializeComponent();
        BindingContext = pageModel;
	}
}