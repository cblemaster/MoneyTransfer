using MoneyTransfer.Core.DTO;
using MoneyTransfer.UI.MAUI.Services.User;
using System.Collections.ObjectModel;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace MoneyTransfer.UI.MAUI.Services.Data
{
    public class HttpDataService : IDataService
    {
        private readonly HttpClient _client;
        private const string BASE_URI = "https://localhost:7144";

        public HttpDataService() => _client = new HttpClient
        {
            BaseAddress = new Uri(BASE_URI)
        };


        public async Task ApproveTransferRequestAsync(int transferId, TransferDetailsDTO transfer)
        {
            StringContent content = new(JsonSerializer.Serialize(transfer));
            content.Headers.ContentType = new("application/json");

            try
            {
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AuthenticatedUserService.GetToken());
                HttpResponseMessage response = await _client.PutAsync($"/Transfer/Approve/{transferId}", content);
                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.PaymentRequired)
                    {
                        return;
                    }
                }
                response.EnsureSuccessStatusCode();
            }
            catch (Exception) { throw; }
        }

        public async Task<AccountDetailsDTO> GetAccountDetailsForUserAsync(int userId)
        {
            try
            {
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AuthenticatedUserService.GetToken());
                HttpResponseMessage response = await _client.GetAsync($"/User/Account/Details/{userId}");
                return response.IsSuccessStatusCode && response.Content is not null
                    ? await response.Content.ReadFromJsonAsync<AccountDetailsDTO>() ?? AccountDetailsDTO.AccountNotFound
                    : response.StatusCode == System.Net.HttpStatusCode.Unauthorized ? AccountDetailsDTO.AccountUserNotAuthorized : AccountDetailsDTO.AccountHttpResponseUnsuccessful;
            }
            catch (Exception) { throw; }
        }

        public async Task<ReadOnlyCollection<TransferDetailsDTO?>> GetCompletedTransfersForUserAsync(int userId)
        {
            try
            {
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AuthenticatedUserService.GetToken());
                HttpResponseMessage response = await _client.GetAsync($"/User/Transfer/Completed/{userId}");
                return response.IsSuccessStatusCode && response.Content is not null
                    ? new ReadOnlyCollection<TransferDetailsDTO?>(response.Content.ReadFromJsonAsAsyncEnumerable<TransferDetailsDTO?>().ToBlockingEnumerable().ToList()) ?? new ReadOnlyCollection<TransferDetailsDTO?>(new List<TransferDetailsDTO?> { TransferDetailsDTO.TransferNotFound })
                    : response.StatusCode == System.Net.HttpStatusCode.Unauthorized ? new ReadOnlyCollection<TransferDetailsDTO?>(new List<TransferDetailsDTO?> { TransferDetailsDTO.TransferUserNotAuthorized }) : new ReadOnlyCollection<TransferDetailsDTO?>(new List<TransferDetailsDTO?> { TransferDetailsDTO.TransferHttpResponseUnsuccessful });
            }
            catch (Exception) { throw; }
        }

        public async Task<ReadOnlyCollection<TransferDetailsDTO?>> GetPendingTransfersForUserAsync(int userId)
        {
            try
            {
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AuthenticatedUserService.GetToken());
                HttpResponseMessage response = await _client.GetAsync($"/User/Transfer/Pending/{userId}");
                return response.IsSuccessStatusCode && response.Content is not null
                    ? new ReadOnlyCollection<TransferDetailsDTO?>(response.Content.ReadFromJsonAsAsyncEnumerable<TransferDetailsDTO?>().ToBlockingEnumerable().ToList()) ?? new ReadOnlyCollection<TransferDetailsDTO?>(new List<TransferDetailsDTO?> { TransferDetailsDTO.TransferNotFound })
                    : response.StatusCode == System.Net.HttpStatusCode.Unauthorized ? new ReadOnlyCollection<TransferDetailsDTO?>(new List<TransferDetailsDTO?> { TransferDetailsDTO.TransferUserNotAuthorized }) : new ReadOnlyCollection<TransferDetailsDTO?>(new List<TransferDetailsDTO?> { TransferDetailsDTO.TransferHttpResponseUnsuccessful });
            }
            catch (Exception) { throw; }
        }

        public async Task<TransferDetailsDTO> GetTransferDetailsAsync(int transferId)
        {
            try
            {
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AuthenticatedUserService.GetToken());
                HttpResponseMessage response = await _client.GetAsync($"/Transfer/Details/{transferId}");
                return response.IsSuccessStatusCode && response.Content is not null
                    ? await response.Content.ReadFromJsonAsync<TransferDetailsDTO>() ?? TransferDetailsDTO.TransferNotFound
                    : response.StatusCode == System.Net.HttpStatusCode.Unauthorized ? TransferDetailsDTO.TransferUserNotAuthorized : TransferDetailsDTO.TransferHttpResponseUnsuccessful;
            }
            catch (Exception) { throw; }
        }

        public async Task RejectTransferRequestAsync(int transferId, TransferDetailsDTO transfer)
        {
            StringContent content = new(JsonSerializer.Serialize(transfer));
            content.Headers.ContentType = new("application/json");

            try
            {
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AuthenticatedUserService.GetToken());
                HttpResponseMessage response = await _client.PutAsync($"/Transfer/Reject/{transferId}", content);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception) { throw; }
        }

        public async Task RequestTransferAsync(string userFromName, string userToName, decimal amount)
        {
            AddTransferDTO transfer = new()
            {
                UserFromName = userFromName,
                UserToName = userToName,
                Amount = amount,
            };

            if (!transfer.Validate().IsValid) { return; }

            StringContent content = new(JsonSerializer.Serialize(transfer));
            content.Headers.ContentType = new("application/json");

            try
            {
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AuthenticatedUserService.GetToken());
                HttpResponseMessage response = await _client.PostAsync("/Transfer/Request", content);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception) { throw; }
        }

        public async Task SendTransferAsync(string userFromName, string userToName, decimal amount)
        {
            AddTransferDTO transfer = new()
            {
                UserFromName = userFromName,
                UserToName = userToName,
                Amount = amount,
            };

            if (!transfer.Validate().IsValid) { return; }

            StringContent content = new(JsonSerializer.Serialize(transfer));
            content.Headers.ContentType = new("application/json");

            try
            {
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AuthenticatedUserService.GetToken());
                HttpResponseMessage response = await _client.PostAsync("/Transfer/Send", content);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception) { throw; }
        }
    }
}
