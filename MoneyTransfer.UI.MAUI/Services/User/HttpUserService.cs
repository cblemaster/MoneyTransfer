using System.Collections.ObjectModel;
using System.Net.Http.Json;
using System.Text.Json;

namespace MoneyTransfer.UI.MAUI.Services.User
{
    public class HttpUserService : IUserService
    {
        private readonly HttpClient _client;
        private const string BASE_URI = "https://localhost:7144";

        public HttpUserService()
        {
            _client = new HttpClient();
            _client.BaseAddress = new Uri(BASE_URI);
        }

        public async Task<UserDTO> GetUserById(int userId)
        {
            try
            {
                HttpResponseMessage response = await _client.GetAsync($"/User/{userId}");
                return response.IsSuccessStatusCode && response.Content is not null
                    ? await response.Content.ReadFromJsonAsync<UserDTO>() ?? Helpers.UserDTONotFound
                    : Helpers.UserDTONotFound;
            }
            catch (Exception) { throw; }
        }

        public async Task<ReadOnlyCollection<User>> GetUsers()
        {
            try
            {
                HttpResponseMessage response = await _client.GetAsync($"/User/GetUsers");
                return response.IsSuccessStatusCode && response.Content is not null
                    ? new ReadOnlyCollection<User>((response.Content.ReadFromJsonAsAsyncEnumerable<User>()!).ToBlockingEnumerable<User>().ToList()) ?? new ReadOnlyCollection<User>(new List<User> { Helpers.UserNotFound })
                    : new ReadOnlyCollection<User>(new List<User> { Helpers.UserNotFound });
            }
            catch (Exception) { throw; }
        }

        public async Task<ReadOnlyCollection<User>> GetUsersNotLoggedIn()
        {
            try
            {
                HttpResponseMessage response = await _client.GetAsync($"/User/GetUsers");
                return response.IsSuccessStatusCode && response.Content is not null
                    ? new ReadOnlyCollection<User>((response.Content.ReadFromJsonAsAsyncEnumerable<User>()!).ToBlockingEnumerable<User>().Where(user => user.Id != AuthenticatedUserService.GetUserId()).ToList()) ?? new ReadOnlyCollection<User>(new List<User> { Helpers.UserNotFound })
                    : new ReadOnlyCollection<User>(new List<User> { Helpers.UserNotFound });
            }
            catch (Exception) { throw; }
        }

        public async Task<UserDTO> LogIn(LogInUser logInUser)
        {
            if (!Helpers.LogInUserIsValid(logInUser)) { return Helpers.UserDTONotValid; }

            StringContent content = new(JsonSerializer.Serialize(logInUser));
            content.Headers.ContentType = new("application/json");

            try
            {
                HttpResponseMessage response = await _client.PostAsync($"/User/LogIn", content);
                response.EnsureSuccessStatusCode();

                return response.Content is not null
                    ? await response.Content.ReadFromJsonAsync<UserDTO>() ?? Helpers.UserDTONotFound
                    : Helpers.UserDTONotFound;
            }
            catch (Exception) { throw; }
        }

        public async Task<bool> Register(LogInUser registerUser)
        {
            if (!Helpers.LogInUserIsValid(registerUser)) { return false; }

            StringContent content = new(JsonSerializer.Serialize(registerUser));
            content.Headers.ContentType = new("application/json");

            try
            {
                HttpResponseMessage response = await _client.PostAsync($"/User/Register", content);
                response.EnsureSuccessStatusCode();

                return response.Content is not null;
            }
            catch (Exception) { throw; }
        }
    }
}
