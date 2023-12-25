using System.Collections.ObjectModel;
using System.Net.Http.Json;
using System.Text.Json;

namespace MoneyTransfer.UI.MAUI.Services.User
{
    public class HttpUserService : IUserService
    {
        private readonly HttpClient _client;
        private const string BASE_URI = "https://localhost:7144";
        public static UserDTO _user = default!;

        public HttpUserService()
        {
            _client = new HttpClient();
            Uri = new(BASE_URI);
        }

        private Uri Uri { get; set; }

        public async Task<UserDTO> GetUserById(int userId)
        {
            if (userId <= 0) { return Helpers.UserDTOSearchParamNotValid; }

            Uri = new($"{BASE_URI}/User/{userId}");
            try
            {
                HttpResponseMessage response = await _client.GetAsync(Uri);
                return response.IsSuccessStatusCode && response.Content is not null
                    ? await response.Content.ReadFromJsonAsync<UserDTO>() ?? Helpers.UserDTONotFound
                    : Helpers.UserDTONotFound;
            }
            catch (Exception) { throw; }
        }

        public async Task<ReadOnlyCollection<User>> GetUsers()
        {
            //_client.Authenticator = new JwtAuthenticator(GetToken());

            Uri = new($"{BASE_URI}/User/GetUsers");
            try
            {
                HttpResponseMessage response = await _client.GetAsync(Uri);
                return response.IsSuccessStatusCode && response.Content is not null
                    ? new ReadOnlyCollection<User>((response.Content.ReadFromJsonAsAsyncEnumerable<User>()!).ToBlockingEnumerable<User>().ToList()) ?? new ReadOnlyCollection<User>(new List<User> { Helpers.UserNotFound })
                    : new ReadOnlyCollection<User>(new List<User> { Helpers.UserNotFound });
            }
            catch (Exception) { throw; }
        }

        public async Task<ReadOnlyCollection<User>> GetUsersNotLoggedIn()
        {
            if (GetUserId() <= 0) { return new ReadOnlyCollection<User>(new List<User> { Helpers.UserSearchParamNotValid }); }

            Uri = new($"{BASE_URI}/User/GetUsers");
            try
            {
                HttpResponseMessage response = await _client.GetAsync(Uri);
                return response.IsSuccessStatusCode && response.Content is not null
                    ? new ReadOnlyCollection<User>((response.Content.ReadFromJsonAsAsyncEnumerable<User>()!).ToBlockingEnumerable<User>().Where(user => user.Id != GetUserId()).ToList()) ?? new ReadOnlyCollection<User>(new List<User> { Helpers.UserNotFound })
                    : new ReadOnlyCollection<User>(new List<User> { Helpers.UserNotFound });
            }
            catch (Exception) { throw; }
        }

        public async Task<UserDTO> LogIn(LogInUser logInUser)
        {
            //TODO: Validation

            Uri = new($"{BASE_URI}/User/LogIn");
            StringContent content = new(JsonSerializer.Serialize(logInUser));
            content.Headers.ContentType = new("application/json");
            try
            {
                HttpResponseMessage response = await _client.PostAsync(Uri, content);
                response.EnsureSuccessStatusCode();

                return response.IsSuccessStatusCode && response.Content is not null
                    ? await response.Content.ReadFromJsonAsync<UserDTO>() ?? Helpers.UserDTONotFound
                    : Helpers.UserDTONotFound;
            }
            catch (Exception) { throw; }
        }

        public static void LogOut() => _user = null!;

        public async Task<bool> Register(LogInUser registerUser)
        {
            //TODO: Validation

            Uri = new($"{BASE_URI}/User/Register");
            StringContent content = new(JsonSerializer.Serialize(registerUser));
            content.Headers.ContentType = new("application/json");
            try
            {
                HttpResponseMessage response = await _client.PostAsync(Uri, content);
                response.EnsureSuccessStatusCode();

                return response.IsSuccessStatusCode && response.Content is not null
                    ? true : false;
            }
            catch (Exception) { throw; }
        }        

        public static string GetToken()
        {
            return _user?.Token ?? string.Empty;
        }

        public int GetUserId() => _user.Id;

        public static bool IsLoggedIn()
        {
            return !string.IsNullOrWhiteSpace(_user.Token);
        }

        public static void SetLogin(UserDTO user)
        {
            _user = user;
        }
    }
}
