using MoneyTransfer.UI.MAUI.PageModels;

namespace MoneyTransfer.UI.MAUI.Pages;

public partial class PendingTransfersPage : ContentPage
{
	public PendingTransfersPage(PendingTransfersPageModel pageModel)
	{
		InitializeComponent();
        BindingContext = pageModel;
	}
}