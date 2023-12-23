using System.Collections.ObjectModel;
using System.Net.Http.Json;

namespace MoneyTransfer.UI.MAUI.Services
{
    internal class MockHttpUserService : IMockUserService
    {
        private readonly HttpClient _client;
        private const string BASE_URI = "https://localhost:7144";

        public MockHttpUserService()
        {
            _client = new HttpClient();
            Uri = new(BASE_URI);
        }

        private Uri Uri { get; set; }

        public async Task<ReadOnlyCollection<User>?> AllUsersNotLoggedIn(User loggedInUser)
        {
            if (loggedInUser is null || loggedInUser.Id <= 0) { return null!; }

            Uri = new($"{BASE_URI}//User/NotLoggedIn");
            try
            {
                HttpResponseMessage response = await _client.GetAsync(Uri);
                return response.IsSuccessStatusCode && response.Content is not null
                    ? new ReadOnlyCollection<User>((response.Content.ReadFromJsonAsAsyncEnumerable<User>()!).ToBlockingEnumerable<User>().ToList())
                    : null;
            }
            catch (Exception) { throw; }
        }

        public async Task<User?> GetLoggedInUserAsync()
        {
            Uri = new($"{BASE_URI}/User/LoggedIn");
            try
            {
                HttpResponseMessage response = await _client.GetAsync(Uri);
                return response.IsSuccessStatusCode && response.Content is not null
                    ? await response.Content.ReadFromJsonAsync<User>()
                    : null;
            }
            catch (Exception) { throw; }
        }
    }
}
