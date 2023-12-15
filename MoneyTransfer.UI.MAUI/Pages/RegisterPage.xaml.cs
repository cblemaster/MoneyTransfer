using MoneyTransfer.UI.MAUI.PageModels;

namespace MoneyTransfer.UI.MAUI.Pages;

public partial class RegisterPage : ContentPage
{
	public RegisterPage(RegisterPageModel pageModel)
	{
		InitializeComponent();
        BindingContext = pageModel;
	}
}