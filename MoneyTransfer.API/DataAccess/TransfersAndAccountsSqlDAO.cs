using Microsoft.Data.SqlClient;
using MoneyTransfer.API.Entities;
using System.Data;

namespace MoneyTransfer.API.DataAccess
{
    public class TransfersAndAccountsSqlDAO : ITransfersAndAccountsDAO
    {
        private readonly string _connectionString;

        public TransfersAndAccountsSqlDAO(string connectionString) =>
            _connectionString = connectionString;

        public void ApproveTransferRequest() => throw new NotImplementedException();
        public async Task<Account> GetAccountDetailsForUserAsync(string username)
        {
            using (SqlConnection connection = new(_connectionString))
            {
                SqlCommand command = new("GetAccountDetailsForUser", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@username", SqlDbType.VarChar, 50).Value = username;

                connection.Open();

                try
                {
                    SqlDataReader reader = await command.ExecuteReaderAsync();

                    return GetAccountFromReader(reader);
                }
                catch (Exception) { throw; }
            }
        }

        public async Task<List<Transfer>> GetCompletedTransfersForUserAsync(string username)
        {
            using (SqlConnection connection = new(_connectionString))
            {
                SqlCommand command = new("GetCompletedTransfersForUser", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@username", SqlDbType.VarChar, 50).Value = username;

                connection.Open();

                try
                {
                    SqlDataReader reader = await command.ExecuteReaderAsync();

                    return GetTransfersFromReader(reader);
                }
                catch (Exception) { throw; }
            }
        }

        public async Task<List<Transfer>> GetPendingTransfersForUserAsync(string username)
        {
            using (SqlConnection connection = new(_connectionString))
            {
                SqlCommand command = new("GetPendingTransfersForUser", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@username", SqlDbType.VarChar, 50).Value = username;

                connection.Open();

                try
                {
                    SqlDataReader reader = await command.ExecuteReaderAsync();

                    return GetTransfersFromReader(reader);
                }
                catch (Exception) { throw; }
            }
        }

        public async Task<Transfer> GetTransferDetailsAsync(int transferId)
        {
            using (SqlConnection connection = new(_connectionString))
            {
                SqlCommand command = new("GetTransferDetails", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@transferId", SqlDbType.Int).Value = transferId;

                connection.Open();

                try
                {
                    SqlDataReader reader = await command.ExecuteReaderAsync();

                    return GetTransferFromReader(reader);
                }
                catch (Exception) { throw; }
            }
        }

        public void RejectTransferRequest() => throw new NotImplementedException();
        public void RequestTransfer() => throw new NotImplementedException();
        public void SendTransfer() => throw new NotImplementedException();

        private static Transfer GetTransferFromReader(SqlDataReader reader)
        {
            if (reader.HasRows && reader.Read())
            {
                int transferId = reader.GetInt32(reader.GetOrdinal("Transfer Id"));
                string userFromName = reader.GetString(reader.GetOrdinal("User From")) ?? "error reading data";
                string userToName = reader.GetString(reader.GetOrdinal("User To")) ?? "error reading data";
                string transferStatus = reader.GetString(reader.GetOrdinal("Transfer Status")) ?? "error reading data";
                string transferType = reader.GetString(reader.GetOrdinal("Transfer Type")) ?? "error reading data";
                decimal transferAmount = reader.GetDecimal(reader.GetOrdinal("Transfer Amount"));
                DateOnly transferDate = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("Transfer Date")));

                if (transferId > 0)
                {
                    return new Transfer(transferId, transferDate, transferAmount, transferStatus, transferType, userToName, userFromName);
                }
            }

            return Transfer.NotFound;
        }

        private static List<Transfer> GetTransfersFromReader(SqlDataReader reader)
        {
            List<Transfer> transfers = new();
            while (reader.HasRows && reader.Read())
            {
                transfers.Add(GetTransferFromReader(reader));
            }

            return transfers;
        }

        private static Account GetAccountFromReader(SqlDataReader reader)
        {
            if (reader.HasRows && reader.Read())
            {
                int accountId = reader.GetInt32(reader.GetOrdinal("Id"));
                string username = reader.GetString(reader.GetOrdinal("Username")) ?? "error reading data";
                decimal currentBalance = reader.GetDecimal(reader.GetOrdinal("Current Balance"));                
                
                if (accountId > 0)
                {
                    return new Account(accountId,username, currentBalance);
                }
            }

            return Account.NotFound;
        }
    }
}
