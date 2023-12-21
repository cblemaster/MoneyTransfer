
using MoneyTransfer.UI.MAUI.Pages;

namespace MoneyTransfer.UI.MAUI
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            RegisterPageRoutes();
        }

        private void RegisterPageRoutes()
        {
            Routing.RegisterRoute("ApproveTransfer", typeof(ApproveTransferRequestPage));
            Routing.RegisterRoute("RejectTransfer", typeof(RejectTransferRequestPage));
            Routing.RegisterRoute("TransferDetails", typeof(TransferDetailsPage));
        }
    }
}
