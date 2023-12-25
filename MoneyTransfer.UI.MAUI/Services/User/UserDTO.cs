namespace MoneyTransfer.UI.MAUI.Services.User
{
    public class UserDTO
    {
        public required int Id { get; init; }
        public required string Username { get; init; }
        public required string Token { get; init; }
        public required string Message { get; init; }
    }
}
