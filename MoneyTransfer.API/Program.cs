using Microsoft.EntityFrameworkCore;
using MoneyTransfer.API.Context;
using MoneyTransfer.API.Entities;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
            .Build();

string connectionString = config.GetConnectionString("Project") ?? "Error retrieving connection string!";

builder.Services
    .AddDbContext<MoneyTransferContext>(options =>
        options.UseSqlServer(connectionString))
    .ConfigureHttpJsonOptions(options =>
        options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapGet("/Transfer/Details/{id}", async (int id, MoneyTransferContext context) =>
{
    return await context.Transfers.Select(t =>
        new Transfer
        {
            Id = t.Id,
            Amount = t.Amount,
            TransferStatusId = t.TransferStatusId,
            TransferTypeId = t.TransferTypeId,
            DateCreated = t.DateCreated,
            AccountIdFromNavigation = new Account
            {
                Id = t.AccountIdFromNavigation.Id,
                User = new User
                {
                    Id = t.AccountIdFromNavigation.User.Id,
                    Username = t.AccountIdFromNavigation.User.Username
                },
            },
            AccountIdToNavigation = new Account
            {
                Id = t.AccountIdToNavigation.Id,
                User = new User
                {
                    Id = t.AccountIdToNavigation.User.Id,
                    Username = t.AccountIdToNavigation.User.Username,
                },
            },
        })
        .SingleOrDefaultAsync(transfer => transfer.Id == id) is Transfer transfer
            ? Results.Ok(transfer)
            : Results.NotFound();
});

app.MapGet("/User/Account/Details/{id}", async (int id, MoneyTransferContext context) =>
    {
        return await context.Accounts.Where(a => a.User.Id == id).Select(a =>
            new Account
            {
                Id = a.Id,
                StartingBalance = a.StartingBalance,
                DateCreated = a.DateCreated,
                TransferAccountIdFromNavigations =
                    a.TransferAccountIdFromNavigations
                        .Select(t => new Transfer
                        {
                            Id = t.Id,
                            Amount = t.Amount,
                            TransferStatusId = t.TransferStatusId,
                        }).ToList(),
                TransferAccountIdToNavigations =
                    a.TransferAccountIdToNavigations
                        .Select(t => new Transfer
                        {
                            Id = t.Id,
                            Amount = t.Amount,
                            TransferStatusId = t.TransferStatusId,
                        }).ToList(),
                User = new User
                {
                    Id = a.User.Id,
                    Username = a.User.Username
                },
            })
            .Select(a => 
                new
                {
                    Id = a.Id,
                    Username = a.User.Username,
                    Balance = a.CurrentBalance(),
                    DateCreated = a.DateCreated,
                }).SingleOrDefaultAsync() is object account            
        ? Results.Ok(account)
        : Results.NotFound();
    });

app.MapGet("/User/Transfer/Completed/{id}", async (int id, MoneyTransferContext context) =>
    {
        return await context.Transfers.Where(transfer =>
            (transfer.AccountIdFromNavigation.UserId == id
              || transfer.AccountIdToNavigation.UserId == id)
              && transfer.TransferStatusId != (int)TransferStatus.Pending)
            .Select(t => new Transfer
            {
                Id = t.Id,
                Amount = t.Amount,
                TransferStatusId = t.TransferStatusId,
                TransferTypeId = t.TransferTypeId,
                DateCreated = t.DateCreated,
                AccountIdFromNavigation = new Account
                {
                    Id = t.AccountIdFromNavigation.Id,
                    User = new User
                    {
                        Id = t.AccountIdFromNavigation.User.Id,
                        Username = t.AccountIdFromNavigation.User.Username
                    },
                },
                AccountIdToNavigation = new Account
                {
                    Id = t.AccountIdToNavigation.Id,
                    User = new User
                    {
                        Id = t.AccountIdToNavigation.User.Id,
                        Username = t.AccountIdToNavigation.User.Username,
                    },
                },
            })
            .ToListAsync();
    });

app.MapGet("/User/Transfer/Pending/{id}", async (int id, MoneyTransferContext context) =>
    {
        return await context.Transfers.Where(transfer =>
            (transfer.AccountIdFromNavigation.UserId == id
              || transfer.AccountIdToNavigation.UserId == id)
              && transfer.TransferStatusId == (int)TransferStatus.Pending)
            .Select(t => new Transfer
            {
                Id = t.Id,
                Amount = t.Amount,
                TransferStatusId = t.TransferStatusId,
                TransferTypeId = t.TransferTypeId,
                DateCreated = t.DateCreated,
                AccountIdFromNavigation = new Account
                {
                    Id = t.AccountIdFromNavigation.Id,
                    User = new User
                    {
                        Id = t.AccountIdFromNavigation.User.Id,
                        Username = t.AccountIdFromNavigation.User.Username
                    },
                },
                AccountIdToNavigation = new Account
                {
                    Id = t.AccountIdToNavigation.Id,
                    User = new User
                    {
                        Id = t.AccountIdToNavigation.User.Id,
                        Username = t.AccountIdToNavigation.User.Username,
                    },
                },
            })
            .ToListAsync();
    });

app.MapPost("/Transfer/Create", async (Transfer transfer, MoneyTransferContext context) =>
    {
        if (!transfer.IsValid()) { return Results.BadRequest(); }

        context.Transfers.Add(transfer);
        await context.SaveChangesAsync();
        return Results.Created($"/Transfer/Details/{transfer.Id}", transfer);
    });

app.MapPut("/Transfer/Approve/{id}", async (int id, Transfer transfer, MoneyTransferContext context) =>
    {
        Transfer findTransfer = (await context.Transfers.FindAsync(id))!;
        if (findTransfer is null) { return Results.NotFound(); }
        if (findTransfer.TransferStatus != TransferStatus.Pending) { return Results.BadRequest(); }

        findTransfer.TransferStatusId = (int)TransferStatus.Approved;
        await context.SaveChangesAsync();
        return Results.NoContent();
    });

app.MapPut("/Transfer/Reject/{id}", async (int id, Transfer transfer, MoneyTransferContext context) =>
    {
        Transfer findTransfer = (await context.Transfers.FindAsync(id))!;
        if (findTransfer is null) { return Results.NotFound(); }
        if (findTransfer.TransferStatus != TransferStatus.Pending) { return Results.BadRequest(); }

        findTransfer.TransferStatusId = (int)TransferStatus.Rejected;
        await context.SaveChangesAsync();
        return Results.NoContent();
    });

app.Run();
