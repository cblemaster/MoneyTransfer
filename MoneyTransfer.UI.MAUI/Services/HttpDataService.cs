using MoneyTransfer.UI.MAUI.Services.Models;
using System.Collections.ObjectModel;
using System.Net.Http.Json;
using System.Text.Json;

namespace MoneyTransfer.UI.MAUI.Services
{
    public class HttpDataService : IDataService
    {
        private readonly HttpClient _client;
        private const string BASE_URI = "https://localhost:7144";

        public HttpDataService()
        {
            _client = new HttpClient();
            Uri = new(BASE_URI);
        }

        private Uri Uri { get; set; }

        public async Task ApproveTransferRequestAsync(int transferId, TransferDetails transfer)
        {
            if (transferId <= 0) { return; }

            Uri = new($"{BASE_URI}/Transfer/Approve/{transferId}");
            StringContent content = new(JsonSerializer.Serialize(transfer));
            content.Headers.ContentType = new("application/json");

            try
            {
                HttpResponseMessage response = await _client.PutAsync(Uri, content);
                if (!response.IsSuccessStatusCode) { throw new HttpRequestException(); }  //TODO: Is this the right exception to throw here?
            }
            catch (Exception) { throw; }
        }

        public async Task<AccountDetails> GetAccountDetailsForUserAsync(int userId)
        {
            if (userId <= 0) { return Helpers.AccountSearchParamNotValid; }

            Uri = new($"{BASE_URI}/User/Account/Details/{userId}");
            try
            {
                HttpResponseMessage response = await _client.GetAsync(Uri);
                return response.IsSuccessStatusCode && response.Content is not null
                    ? await response.Content.ReadFromJsonAsync<AccountDetails>() ?? Helpers.AccountNotFound
                    : Helpers.AccountNotFound;
            }
            catch (Exception) { throw; }
        }

        public async Task<ReadOnlyCollection<TransferDetails>> GetCompletedTransfersForUserAsync(int userId)
        {
            if (userId <= 0)
            {
                return new ReadOnlyCollection<TransferDetails>(new List<TransferDetails> { Helpers.TransferSearchParamNotValid });
            }

            Uri = new($"{BASE_URI}/User/Transfer/Completed/{userId}");
            try
            {
                HttpResponseMessage response = await _client.GetAsync(Uri);
                return response.IsSuccessStatusCode && response.Content is not null
                    ? new ReadOnlyCollection<TransferDetails>((response.Content.ReadFromJsonAsAsyncEnumerable<TransferDetails>()!).ToBlockingEnumerable<TransferDetails>().ToList()) ?? new ReadOnlyCollection<TransferDetails>(new List<TransferDetails> { Helpers.TransferNotFound })
                    : new ReadOnlyCollection<TransferDetails>(new List<TransferDetails> { Helpers.TransferNotFound });
            }
            catch (Exception) { throw; }
        }

        public async Task<ReadOnlyCollection<TransferDetails>> GetPendingTransfersForUserAsync(int userId)
        {
            if (userId <= 0)
            {
                return new ReadOnlyCollection<TransferDetails>(new List<TransferDetails> { Helpers.TransferSearchParamNotValid });
            }

            Uri = new($"{BASE_URI}/User/Transfer/Pending/{userId}");
            try
            {
                HttpResponseMessage response = await _client.GetAsync(Uri);
                return response.IsSuccessStatusCode && response.Content is not null
                    ? new ReadOnlyCollection<TransferDetails>((response.Content.ReadFromJsonAsAsyncEnumerable<TransferDetails>()!).ToBlockingEnumerable<TransferDetails>().ToList()) ?? new ReadOnlyCollection<TransferDetails>(new List<TransferDetails> { Helpers.TransferNotFound })
                    : new ReadOnlyCollection<TransferDetails>(new List<TransferDetails> { Helpers.TransferNotFound });
            }
            catch (Exception) { throw; }
        }

        public async Task<TransferDetails> GetTransferDetailsAsync(int transferId)
        {
            if (transferId <= 0) { return Helpers.TransferSearchParamNotValid; }

            Uri = new($"{BASE_URI}/Transfer/Details/{transferId}");
            try
            {
                HttpResponseMessage response = await _client.GetAsync(Uri);
                return response.IsSuccessStatusCode && response.Content is not null
                    ? await response.Content.ReadFromJsonAsync<TransferDetails>() ?? Helpers.TransferNotFound
                    : Helpers.TransferNotFound;
            }
            catch (Exception) { throw; }
        }

        public async Task RejectTransferRequestAsync(int transferId, TransferDetails transfer)
        {
            if (transferId <= 0) { return; }

            Uri = new($"{BASE_URI}/Transfer/Reject/{transferId}");
            StringContent content = new(JsonSerializer.Serialize(transfer));
            content.Headers.ContentType = new("application/json");

            try
            {
                HttpResponseMessage response = await _client.PutAsync(Uri, content);
                if (!response.IsSuccessStatusCode) { throw new HttpRequestException(); }  //TODO: Is this the right exception to throw here?
            }
            catch (Exception) { throw; }
        }

        public async Task RequestTransferAsync(string userFromName, string userToName, decimal amount)
        {
            AddTransfer transfer = new()
            {
                UserFromName = userFromName,
                UserToName = userToName,
                Amount = amount,
            };

            if (!Helpers.AddTransferIsValid(transfer)) { return; }

            Uri = new($"{BASE_URI}/Transfer/Request");
            try
            {
                StringContent content = new(JsonSerializer.Serialize(transfer));
                content.Headers.ContentType = new("application/json");

                HttpResponseMessage response = await _client.PostAsync(Uri, content);
                if (!response.IsSuccessStatusCode) { throw new HttpRequestException(); }  //TODO: Is this the right exception to throw here?
            }
            catch (Exception) { throw; }
        }

        public async Task SendTransferAsync(string userFromName, string userToName, decimal amount)
        {
            AddTransfer transfer = new()
            {
                UserFromName = userFromName,
                UserToName = userToName,
                Amount = amount,
            };

            if (!Helpers.AddTransferIsValid(transfer)) { return; }

            Uri = new($"{BASE_URI}/Transfer/Send");
            try
            {
                StringContent content = new(JsonSerializer.Serialize(transfer));
                content.Headers.ContentType = new("application/json");

                HttpResponseMessage response = await _client.PostAsync(Uri, content);
                if (!response.IsSuccessStatusCode) { throw new HttpRequestException(); }  //TODO: Is this the right exception to throw here?
            }
            catch (Exception) { throw; }
        }
    }
}
