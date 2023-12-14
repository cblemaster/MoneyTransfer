using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace MoneyTransfer.API.DataAccess
{
    public partial class MoneyTransferContext : DbContext
    {
        public MoneyTransferContext()
        {

        }

        public MoneyTransferContext(DbContextOptions<MoneyTransferContext> options)
            : base(options)
        {

        }

        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<Transfer> Transfers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) =>
            OnModelCreatingPartial(modelBuilder);

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

        public void ApproveTransferRequest(int transferId) =>
            Database.ExecuteSqlRaw("dbo.ApproveTransferRequest @transferId int",
                new SqlParameter("@transferId int", transferId));

        public Account GetAccountDetailsForUser(string username) =>
            Accounts
            .FromSql($"GetAccountDetailsForUser {username}")
            .SingleOrDefault(Account.NotFound);

        public List<Transfer> GetCompletedTransfersForUser(string username) =>
            Transfers
            .FromSql($"GetCompletedTransfersForUser {username}")
            .ToList();

        public List<Transfer> GetPendingTransfersForUser(string username) =>
            Transfers
            .FromSql($"GetPendingTransfersForUser {username}")
            .ToList();

        public Transfer GetTransferDetails(int transferId) =>
            Transfers
            .FromSql($"GetTransferDetails {transferId}")
            .SingleOrDefault(Transfer.NotFound);

        public void RejectTransferRequest(int transferId) =>
            Database.ExecuteSqlRaw("dbo.RejectTransferRequest @transferId int",
                new SqlParameter("@transferId int", transferId));

        public void RequestTransfer(string userFromName,
            string userToName, decimal amount) =>
                Database.ExecuteSqlRaw("dbo.SendTransfer @userFromName varchar(50), @userToName varchar(50), @amount decimal", new SqlParameter("@userFromName", userFromName), new SqlParameter("@userToName", userToName), new SqlParameter("@amount", amount));

        public void SendTransfer(string userFromName,
            string userToName, decimal amount) =>
                Database.ExecuteSqlRaw("dbo.SendTransfer @userFromName varchar(50), @userToName varchar(50), @amount decimal", new SqlParameter("@userFromName", userFromName), new SqlParameter("@userToName", userToName), new SqlParameter("@amount", amount));
    }
}
