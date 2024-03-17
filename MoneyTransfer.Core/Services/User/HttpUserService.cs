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
                    ? await response.Content.ReadFromJsonAsync<UserDTO>() ?? UserDTO.UserDTONotFound
                    : UserDTO.UserDTONotFound;
            }
            catch (Exception) { throw; }
        }

        public async Task<ReadOnlyCollection<UserDTO>> GetUsers()
        {
            try
            {
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AuthenticatedUserService.GetToken());
                HttpResponseMessage response = await _client.GetAsync($"/User/GetUsers");
                return response.IsSuccessStatusCode && response.Content is not null
                    ? new ReadOnlyCollection<UserDTO>((response.Content.ReadFromJsonAsAsyncEnumerable<UserDTO>()!).ToBlockingEnumerable<UserDTO>().ToList()) ?? new ReadOnlyCollection<UserDTO>(new List<UserDTO> { UserDTO.UserDTONotFound })
                    : new ReadOnlyCollection<UserDTO>(new List<UserDTO> { UserDTO.UserDTONotFound });
            }
            catch (Exception) { throw; }
        }

        public async Task<ReadOnlyCollection<UserDTO>> GetUsersNotLoggedIn()
        {
            try
            {
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AuthenticatedUserService.GetToken());
                HttpResponseMessage response = await _client.GetAsync($"/User/GetUsers");
                return response.IsSuccessStatusCode && response.Content is not null
                    ? new ReadOnlyCollection<UserDTO>((response.Content.ReadFromJsonAsAsyncEnumerable<UserDTO>()!).ToBlockingEnumerable<UserDTO>().Where(user => user.Id != AuthenticatedUserService.GetUserId()).ToList()) ?? new ReadOnlyCollection<UserDTO>(new List<UserDTO> { UserDTO.UserDTONotFound })
                    : new ReadOnlyCollection<UserDTO>(new List<UserDTO> { UserDTO.UserDTONotFound });
            }
            catch (Exception) { throw; }
        }

        public async Task<UserDTO> LogIn(LogInUserDTO logInUser)
        {
            if (!logInUser.Validate().IsValid) { return UserDTO.UserDTONotValid; }

            StringContent content = new(JsonSerializer.Serialize(logInUser));
            content.Headers.ContentType = new("application/json");

            try
            {
                HttpResponseMessage response = await _client.PostAsync($"/User/LogIn", content);
                return !response.IsSuccessStatusCode
                    ? response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                        ? UserDTO.UserDTOUserNotAuthorized
                        : UserDTO.UserDTOHttpResponseUnsuccessful
                    : response.Content is not null
                        ? await response.Content.ReadFromJsonAsync<UserDTO>() ?? UserDTO.UserDTONotFound
                        : UserDTO.UserDTONotFound;
            }
            catch (Exception) { throw; }
        }

        public async Task<bool> Register(LogInUserDTO registerUser)
        {
            if (!registerUser.Validate().IsValid) { return false; }

            StringContent content = new(JsonSerializer.Serialize(registerUser));
            content.Headers.ContentType = new("application/json");

            try
            {
                HttpResponseMessage response = await _client.PostAsync($"/User/Register", content);
                return response.IsSuccessStatusCode && response.Content is not null;
            }
            catch (Exception) { throw; }
        }
    }
}
