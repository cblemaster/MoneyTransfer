using MoneyTransfer.UI.MAUI.PageModels;

namespace MoneyTransfer.UI.MAUI.Pages;

public partial class LogOutPage : ContentPage
{
	public LogOutPage(LogOutPageModel pageModel)
	{
		InitializeComponent();
        BindingContext = pageModel;
	}
}