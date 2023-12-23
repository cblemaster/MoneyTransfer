﻿using MoneyTransfer.UI.MAUI.Services.Models;
using System.Collections.ObjectModel;

namespace MoneyTransfer.UI.MAUI.Services
{
    public interface IMockUserService
    {
        Task<User?> GetLoggedInUserAsync();
        Task<ReadOnlyCollection<User>?> AllUsersNotLoggedIn(User loggedInUser);
    }

    public class User
    {
        public required int Id { get; init; }

        public required string Username { get; init; }

        public string PasswordHash { get; set; } = null!;

        public string Salt { get; set; } = null!;
    }
}