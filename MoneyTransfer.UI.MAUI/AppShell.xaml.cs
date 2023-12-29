using MoneyTransfer.UI.MAUI.PageModels;

namespace MoneyTransfer.UI.MAUI
{
    public partial class AppShell : Shell
    {
        public AppShell(AppShellModel shellModel)
        {
            InitializeComponent();
            BindingContext = shellModel;
        }
    }
}
