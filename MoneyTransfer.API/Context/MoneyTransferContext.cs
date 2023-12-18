using Microsoft.EntityFrameworkCore;
using MoneyTransfer.API.Entities;

namespace MoneyTransfer.API.Context;

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

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasIndex(e => e.UserId, "UC_UserId").IsUnique();

            entity.Property(e => e.StartingBalance).HasColumnType("decimal(13, 2)");

            entity.HasOne(d => d.User).WithOne(p => p.Account)
                .HasForeignKey<Account>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Accounts_Users");
        });

        modelBuilder.Entity<Transfer>(entity =>
        {
            entity.Property(e => e.Amount).HasColumnType("decimal(13, 2)");

            entity.HasOne(d => d.AccountIdFromNavigation).WithMany(p => p.TransferAccountIdFromNavigations)
                .HasForeignKey(d => d.AccountIdFrom)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Transfers_AccountsFrom");

            entity.HasOne(d => d.AccountIdToNavigation).WithMany(p => p.TransferAccountIdToNavigations)
                .HasForeignKey(d => d.AccountIdTo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Transfers_AccountsTo");

            entity.Ignore(e => e.TransferType).Ignore(e => e.TransferStatus);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.Username, "UC_Username").IsUnique();

            entity.Property(e => e.PasswordHash)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Salt)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
