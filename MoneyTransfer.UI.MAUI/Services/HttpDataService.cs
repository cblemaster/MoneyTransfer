using MoneyTransfer.UI.MAUI.Services.Models;
using System.Collections.ObjectModel;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace MoneyTransfer.UI.MAUI.Services
{
    public class HttpDataService : IDataService
    {
        private readonly HttpClient _client;
        private readonly JsonSerializerOptions _serializerOptions;
        private const string BASE_URI = "https://localhost:7144";

        public HttpDataService()
        {
            _client = new HttpClient();
            _serializerOptions = new JsonSerializerOptions();
            Uri = new(BASE_URI);
        }

        private Uri Uri { get; set; }

        public async Task ApproveTransferRequestAsync(int transferId)
        {
            if (transferId <= 0) { return; }

            Uri = new($"{BASE_URI}/Transfer/Approve/{transferId}");
            try
            {
                HttpResponseMessage response = await _client.GetAsync(Uri);
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
                if (response.IsSuccessStatusCode && response.Content is not null)
                {
                    return await response.Content.ReadFromJsonAsync<AccountDetails>() ?? Helpers.AccountNotFound;
                }

                return Helpers.AccountNotFound;
            }
            catch (Exception) { throw; }
        }

        public async Task<IReadOnlyCollection<TransferDetails>> GetCompletedTransfersForUserAsync(int userId)
        {
            if (userId <= 0)
            {
                return new ReadOnlyCollection<TransferDetails>(new List<TransferDetails> { Helpers.TransferSearchParamNotValid });
            }

            Uri = new($"{BASE_URI}/User/Transfer/Completed/{userId}");
            try
            {
                HttpResponseMessage response = await _client.GetAsync(Uri);
                if (response.IsSuccessStatusCode && response.Content is not null)
                {
                    ReadOnlyCollection<TransferDetails> transfersFromResponse = new(JsonSerializer.Deserialize<List<TransferDetails>>
                            (response.Content.ToString()!, _serializerOptions)!);

                    if (transfersFromResponse == null)
                    {
                        return new ReadOnlyCollection<TransferDetails>(new List<TransferDetails> { Helpers.TransferNotFound });
                    }

                    return transfersFromResponse;
                }

                return new ReadOnlyCollection<TransferDetails>(new List<TransferDetails> { Helpers.TransferNotFound });
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
                if (response.IsSuccessStatusCode && response.Content is not null)
                {
                    return new ReadOnlyCollection<TransferDetails>((response.Content.ReadFromJsonAsAsyncEnumerable<TransferDetails>()!).ToBlockingEnumerable<TransferDetails>().ToList()) ?? new ReadOnlyCollection<TransferDetails>(new List<TransferDetails> { Helpers.TransferNotFound });
                }

                return new ReadOnlyCollection<TransferDetails>(new List<TransferDetails> { Helpers.TransferNotFound });
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
                if (response.IsSuccessStatusCode && response.Content is not null)
                {
                    return await response.Content.ReadFromJsonAsync<TransferDetails>() ?? Helpers.TransferNotFound;
                }

                return Helpers.TransferNotFound;
            }
            catch (Exception) { throw; }
        }

        public async Task RejectTransferRequestAsync(int transferId)
        {
            if (transferId <= 0) { return; }

            Uri = new($"{BASE_URI}/Transfer/Reject/{transferId}");
            try
            {
                HttpResponseMessage response = await _client.GetAsync(Uri);
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
                StringContent content = new StringContent(JsonSerializer.Serialize(transfer));
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
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
                StringContent content = new StringContent(JsonSerializer.Serialize(transfer));
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                HttpResponseMessage response = await _client.PostAsync(Uri, content);

                if (!response.IsSuccessStatusCode) { throw new HttpRequestException(); }  //TODO: Is this the right exception to throw here?
            }
            catch (Exception) { throw; }
        }
    }
}
