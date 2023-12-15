using MoneyTransfer.UI.MAUI.PageModels;

namespace MoneyTransfer.UI.MAUI.Pages;

public partial class LogInPage : ContentPage
{
	public LogInPage(LogInPageModel pageModel)
	{
		InitializeComponent();
        BindingContext = pageModel;
	}
}