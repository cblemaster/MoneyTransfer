using MoneyTransfer.Core.DTO;
using System.Collections.ObjectModel;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace MoneyTransfer.UI.MAUI.Services.User
{
    public class HttpUserService : IUserService
    {
        private readonly HttpClient _client;
        private const string BASE_URI = "https://localhost:7144";

        public HttpUserService() => _client = new HttpClient
        {
            BaseAddress = new Uri(BASE_URI)
        };

        public async Task<UserDTO> GetUserById(int userId)
        {
            try
            {
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AuthenticatedUserService.GetToken());
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
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AuthenticatedUserService.GetToken());
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
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AuthenticatedUserService.GetToken());
                HttpResponseMessage response = await _client.GetAsync($"/User/GetUsers");
                return response.IsSuccessStatusCode && response.Content is not null
                    ? new ReadOnlyCollection<User>((response.Content.ReadFromJsonAsAsyncEnumerable<User>()!).ToBlockingEnumerable<User>().Where(user => user.Id != AuthenticatedUserService.GetUserId()).ToList()) ?? new ReadOnlyCollection<User>(new List<User> { Helpers.UserNotFound })
                    : new ReadOnlyCollection<User>(new List<User> { Helpers.UserNotFound });
            }
            catch (Exception) { throw; }
        }

        public async Task<UserDTO> LogIn(LogInUserDTO logInUser)
        {
            if (!Helpers.LogInUserIsValid(logInUser)) { return Helpers.UserDTONotValid; }

            StringContent content = new(JsonSerializer.Serialize(logInUser));
            content.Headers.ContentType = new("application/json");

            try
            {
                HttpResponseMessage response = await _client.PostAsync($"/User/LogIn", content);
                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        return Helpers.UserDTOUserNotAuthorized;
                    }
                    else
                    {
                        return Helpers.UserDTOHttpResponseUnsuccessful;
                    }
                }
                else if (response.Content is not null)
                {
                    return await response.Content.ReadFromJsonAsync<UserDTO>() ?? Helpers.UserDTONotFound;
                }
                else
                {
                    return Helpers.UserDTONotFound;
                }
            }
            catch (Exception) { throw; }
        }

        public async Task<bool> Register(LogInUserDTO registerUser)
        {
            if (!Helpers.LogInUserIsValid(registerUser)) { return false; }

            StringContent content = new(JsonSerializer.Serialize(registerUser));
            content.Headers.ContentType = new("application/json");

            try
            {
                HttpResponseMessage response = await _client.PostAsync($"/User/Register", content);
                if (response.IsSuccessStatusCode && response.Content is not null)
                {
                    return true;
                }
                
                return false;
            }
            catch (Exception) { throw; }
        }
    }
}
