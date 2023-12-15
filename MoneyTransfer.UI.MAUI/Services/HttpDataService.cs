using MoneyTransfer.UI.MAUI.Services.Models;
using System.Text.Json;

namespace MoneyTransfer.UI.MAUI.Services
{
    public class HttpDataService : IDataService
    {
        private readonly HttpClient _client;
        private JsonSerializerOptions _serializerOptions;
        private const string BASE_URI = "https://localhost:7024";

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

            Uri = new($"{BASE_URI}/ApproveTransferRequest/{transferId}");
            try
            {
                HttpResponseMessage response = await _client.GetAsync(Uri);
                if (!response.IsSuccessStatusCode) { throw new HttpRequestException(); }  //TODO: Is this the right exception to throw here?
            }
            catch (Exception) { throw; }
        }

        public async Task<AccountDetails> GetAccountDetailsForUserAsync(string username)
        {
            if (string.IsNullOrEmpty(username) ||
                string.IsNullOrWhiteSpace(username) || 
                username.Length > 50)
                { return AccountDetails.SearchParamNotValid; }

            Uri = new($"{BASE_URI}/GetAccountDetailsForUser/{username}");
            try
            {
                HttpResponseMessage response = await _client.GetAsync(Uri);
                if (response.IsSuccessStatusCode && response.Content is not null)
                {
                    AccountDetails accountFromResponse = 
                        JsonSerializer.Deserialize<AccountDetails>
                            (response.Content.ToString()!, _serializerOptions)!;

                    if (accountFromResponse == null)
                    {
                        return AccountDetails.NotFound;
                    }
                    else if (!accountFromResponse.IsValid())
                    {
                        return AccountDetails.NotValid;
                    }
                    
                    return accountFromResponse;
                }

                return AccountDetails.NotFound;
            }
            catch (Exception) { throw; }
        }

        public async Task<List<TransferDetails>> GetCompletedTransfersForUserAsync(string username)
        {
            if (string.IsNullOrEmpty(username) ||
                string.IsNullOrWhiteSpace(username) ||
                username.Length > 50)
            { return new() { TransferDetails.SearchParamNotValid }; }

            Uri = new($"{BASE_URI}/GetCompletedTransfersForUser/{username}");
            try
            {
                HttpResponseMessage response = await _client.GetAsync(Uri);
                if (response.IsSuccessStatusCode && response.Content is not null)
                {
                    List<TransferDetails> transfersFromResponse = 
                        JsonSerializer.Deserialize<List<TransferDetails>>
                            (response.Content.ToString()!, _serializerOptions)!;
                    
                    if (transfersFromResponse == null || !transfersFromResponse.Any())
                    {
                        return new() { TransferDetails.NotFound };
                    }
                    else if (!transfersFromResponse.All(transfer => transfer.IsValid()))
                    {
                        return new() { TransferDetails.NotValid };
                    }
                    
                    return transfersFromResponse;
                }

                return new() { TransferDetails.NotFound };
            }
            catch (Exception) { throw; }
        }

        public async Task<List<TransferDetails>> GetPendingTransfersForUserAsync(string username)
        {
            if (string.IsNullOrEmpty(username) ||
                string.IsNullOrWhiteSpace(username) ||
                username.Length > 50)
            { return new() { TransferDetails.SearchParamNotValid }; }

            Uri = new($"{BASE_URI}/GetPendingTransfersForUser/{username}");
            try
            {
                HttpResponseMessage response = await _client.GetAsync(Uri);
                if (response.IsSuccessStatusCode && response.Content is not null)
                {
                    List<TransferDetails> transfersFromResponse =
                        JsonSerializer.Deserialize<List<TransferDetails>>
                            (response.Content.ToString()!, _serializerOptions)!;

                    if (transfersFromResponse == null || !transfersFromResponse.Any())
                    {
                        return new() { TransferDetails.NotFound };
                    }
                    else if (!transfersFromResponse.All(transfer => transfer.IsValid()))
                    {
                        return new() { TransferDetails.NotValid };
                    }

                    return transfersFromResponse;
                }

                return new() { TransferDetails.NotFound };
            }
            catch (Exception) { throw; }
        }

        public async Task<TransferDetails> GetTransferDetailsAsync(int transferId)
        {
            if (transferId <= 0) { return TransferDetails.SearchParamNotValid; }

            Uri = new($"{BASE_URI}/GetTransferDetails/{transferId}");
            try
            {
                HttpResponseMessage response = await _client.GetAsync(Uri);
                if (response.IsSuccessStatusCode && response.Content is not null)
                {
                    TransferDetails transferFromResponse =
                        JsonSerializer.Deserialize<TransferDetails>
                            (response.Content.ToString()!, _serializerOptions)!;

                    if (transferFromResponse == null)
                    {
                        return TransferDetails.NotFound;
                    }
                    else if (!transferFromResponse.IsValid())
                    {
                        return TransferDetails.NotValid;
                    }

                    return transferFromResponse;
                }

                return TransferDetails.NotFound;
            }
            catch (Exception) { throw; }
        }

        public async Task RejectTransferRequestAsync(int transferId)
        {
            if (transferId <= 0) { return; }

            Uri = new($"{BASE_URI}/RejectTransferRequest/{transferId}");
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

            if (!transfer.IsValid()) { return; }

            Uri = new($"{BASE_URI}/RequestTransfer/{userFromName}/{userToName}/{amount}");
            try
            {
                HttpResponseMessage response = await _client.GetAsync(Uri);
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

            if (!transfer.IsValid()) { return; }

            Uri = new($"{BASE_URI}/SendTransfer/{userFromName}/{userToName}/{amount}");
            try
            {
                HttpResponseMessage response = await _client.GetAsync(Uri);
                if (!response.IsSuccessStatusCode) { throw new HttpRequestException(); }  //TODO: Is this the right exception to throw here?
            }
            catch (Exception) { throw; }
        }
    }
}
