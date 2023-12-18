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
        return await context.Accounts.Select(a =>
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
                        }).ToList(),
                TransferAccountIdToNavigations =
                    a.TransferAccountIdToNavigations
                        .Select(t => new Transfer
                        {
                            Id = t.Id,
                            Amount = t.Amount,
                        }).ToList(),
                User = new User
                {
                    Id = a.User.Id,
                    Username = a.User.Username
                },
            })
            .SingleOrDefaultAsync(a => a.UserId == id) is Account account
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

//app.MapPost("/Account/Create", async
//    (Account account, MoneyTransferContext context) => {
//        context.Accounts.Add(account);
//        await context.SaveChangesAsync();

//        return Results.Created($"/Account/Get/{account.Id}", account);
//    });

//app.MapPost("/Transfer/Create", async
//    (Transfer transfer, MoneyTransferContext context) => {
//        context.Transfers.Add(transfer);
//        await context.SaveChangesAsync();

//        return Results.Created($"/Transfer/Get/{transfer.Id}", transfer);
//    });

//app.MapPut("/Account/Update/{id}", async
//    (int id, Account account, MoneyTransferContext context) => {
//        Account findAccount = (await context.Accounts.FindAsync(id))!;

//        if (findAccount is null) { return Results.NotFound(); }

//        findAccount.Id = account.Id;
//        findAccount.UserId = account.UserId;
//        findAccount.User = account.User;
//        findAccount.DateCreated = account.DateCreated;
//        findAccount.StartingBalance = account.StartingBalance;
//        findAccount.TransferAccountIdFromNavigations = account.TransferAccountIdFromNavigations;
//        findAccount.TransferAccountIdToNavigations = account.TransferAccountIdToNavigations;

//        await context.SaveChangesAsync();

//        return Results.NoContent();
//    });

//app.MapPut("/Transfer/Update/{id}",
//    async (int id, Transfer transfer, MoneyTransferContext context) => {
//        Transfer findTransfer = (await context.Transfers.FindAsync(id))!;

//        if (findTransfer is null) { return Results.NotFound(); }

//        findTransfer.Id = transfer.Id;
//        findTransfer.Amount = transfer.Amount;
//        findTransfer.AccountIdFrom = transfer.AccountIdFrom;
//        findTransfer.AccountIdTo = transfer.AccountIdTo;
//        findTransfer.AccountIdFromNavigation = transfer.AccountIdFromNavigation;
//        findTransfer.AccountIdToNavigation = transfer.AccountIdToNavigation;
//        findTransfer.TransferStatusId = transfer.TransferStatusId;
//        findTransfer.TransferTypeId = transfer.TransferTypeId;
//        findTransfer.DateCreated = transfer.DateCreated;

//        await context.SaveChangesAsync();

//        return Results.NoContent();
//    });

app.Run();
