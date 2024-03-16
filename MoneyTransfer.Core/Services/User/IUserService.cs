﻿using MoneyTransfer.Core.DTO;
using System.Collections.ObjectModel;

namespace MoneyTransfer.UI.MAUI.Services.User
{
    public interface IUserService
    {
        Task<UserDTO> GetUserById(int userId);
        Task<ReadOnlyCollection<User>> GetUsers();
        Task<ReadOnlyCollection<User>> GetUsersNotLoggedIn();
        Task<UserDTO> LogIn(LogInUserDTO logInUser);
        Task<bool> Register(LogInUserDTO registerUser);
    }
}
