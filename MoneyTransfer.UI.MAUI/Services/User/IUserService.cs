using System.Collections.ObjectModel;

namespace MoneyTransfer.UI.MAUI.Services.User
{
    public interface IUserService
    {
        Task<UserDTO> GetUserById(int userId);
        Task<ReadOnlyCollection<User>> GetUsers();
        Task<ReadOnlyCollection<User>> GetUsersNotLoggedIn();
        Task<UserDTO> LogIn(LogInUser logInUser);
        //void LogOut();
        Task<bool> Register(LogInUser registerUser);
        //string GetToken();
        int GetUserId();
        //bool IsLoggedIn();
        //void SetLogin(UserDTO user);
    }
}
