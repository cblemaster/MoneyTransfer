namespace MoneyTransfer.UI.MAUI.Services.Data
{
    public class AddTransfer
    {
        public required string UserFromName { get; set; }

        public required string UserToName { get; set; }

        public required decimal Amount { get; set; }
    }
}
