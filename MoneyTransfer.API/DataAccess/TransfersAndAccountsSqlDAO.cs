using Microsoft.Data.SqlClient;
using MoneyTransfer.API.Entities;
using System.Data;

namespace MoneyTransfer.API.DataAccess
{
    public class TransfersAndAccountsSqlDAO(string connectionString) : ITransfersAndAccountsDAO
    {
        private readonly string _connectionString = connectionString;

        public async Task ApproveTransferRequestAsync(int transferId)
        {
            using SqlConnection connection = new(_connectionString);
            SqlCommand command = new("ApproveTransferRequest", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@transferId", SqlDbType.Int).Value = transferId;

            connection.Open();

            try
            {
                await command.ExecuteNonQueryAsync();
            }
            catch (Exception) { throw; }
        }

        public async Task<Account> GetAccountDetailsForUserAsync(string username)
        {
            using SqlConnection connection = new(_connectionString);
            SqlCommand command = new("GetAccountDetailsForUser", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@username", SqlDbType.VarChar, 50).Value = username;

            connection.Open();

            try
            {
                SqlDataReader reader = await command.ExecuteReaderAsync();

                return reader.HasRows && reader.Read() ? GetAccountFromReader(reader) : Account.NotFound;
            }
            catch (Exception) { throw; }
        }

        public async Task<List<Transfer>> GetCompletedTransfersForUserAsync(string username)
        {
            using SqlConnection connection = new(_connectionString);
            SqlCommand command = new("GetCompletedTransfersForUser", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@username", SqlDbType.VarChar, 50).Value = username;

            connection.Open();

            try
            {
                SqlDataReader reader = await command.ExecuteReaderAsync();

                return GetTransfersFromReader(reader);
            }
            catch (Exception) { throw; }
        }

        public async Task<List<Transfer>> GetPendingTransfersForUserAsync(string username)
        {
            using SqlConnection connection = new(_connectionString);
            SqlCommand command = new("GetPendingTransfersForUser", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@username", SqlDbType.VarChar, 50).Value = username;

            connection.Open();

            try
            {
                SqlDataReader reader = await command.ExecuteReaderAsync();

                return GetTransfersFromReader(reader);
            }
            catch (Exception) { throw; }
        }

        public async Task<Transfer> GetTransferDetailsAsync(int transferId)
        {
            using SqlConnection connection = new(_connectionString);
            SqlCommand command = new("GetTransferDetails", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@transferId", SqlDbType.Int).Value = transferId;

            connection.Open();

            try
            {
                SqlDataReader reader = await command.ExecuteReaderAsync();

                return reader.HasRows && reader.Read() ? GetTransferFromReader(reader) : Transfer.NotFound;
            }
            catch (Exception) { throw; }
        }

        public async Task RejectTransferRequestAsync(int transferId)
        {
            using SqlConnection connection = new(_connectionString);
            SqlCommand command = new("RejectTransferRequest", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@transferId", SqlDbType.Int).Value = transferId;

            connection.Open();

            try
            {
                await command.ExecuteNonQueryAsync();
            }
            catch (Exception) { throw; }
        }

        public async Task RequestTransferAsync(string userFromName, string userToName, decimal amount)
        {
            using SqlConnection connection = new(_connectionString);
            SqlCommand command = new("RequestTransfer", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@userFromName", SqlDbType.VarChar, 50).Value = userFromName;
            command.Parameters.Add("@userToName", SqlDbType.VarChar, 50).Value = userToName;
            command.Parameters.Add("@amount", SqlDbType.Decimal).Value = amount;

            connection.Open();

            try
            {
                await command.ExecuteNonQueryAsync();
            }
            catch (Exception) { throw; }
        }

        public async Task SendTransferAsync(string userFromName, string userToName, decimal amount)
        {
            using SqlConnection connection = new(_connectionString);
            SqlCommand command = new("SendTransfer", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@userFromName", SqlDbType.VarChar, 50).Value = userFromName;
            command.Parameters.Add("@userToName", SqlDbType.VarChar, 50).Value = userToName;
            command.Parameters.Add("@amount", SqlDbType.Decimal).Value = amount;

            connection.Open();

            try
            {
                await command.ExecuteNonQueryAsync();
            }
            catch (Exception) { throw; }
        }

        private static Transfer GetTransferFromReader(SqlDataReader reader)
        {
            int transferId = reader.GetInt32(reader.GetOrdinal("Transfer Id"));
            string userFromName = reader.GetString(reader.GetOrdinal("User From")) ?? "error reading data";
            string userToName = reader.GetString(reader.GetOrdinal("User To")) ?? "error reading data";
            string transferStatus = reader.GetString(reader.GetOrdinal("Transfer Status")) ?? "error reading data";
            string transferType = reader.GetString(reader.GetOrdinal("Transfer Type")) ?? "error reading data";
            decimal transferAmount = reader.GetDecimal(reader.GetOrdinal("Transfer Amount"));
            DateOnly transferDate = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("Transfer Date")));

            return transferId > 0
                ? new Transfer(transferId, transferDate, transferAmount, transferStatus, transferType, userToName, userFromName)
                : Transfer.NotFound;
        }

        private static List<Transfer> GetTransfersFromReader(SqlDataReader reader)
        {
            List<Transfer> transfers = [];
            while (reader.Read())
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
