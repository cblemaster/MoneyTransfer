namespace MoneyTransfer.UI.MAUI.Services.Models
{
    public class AddTransfer
    {
        public string UserFromName { get; set; } = string.Empty;

        public string UserToName { get; set; } = string.Empty;

        public decimal Amount { get; set; }
    }
}
