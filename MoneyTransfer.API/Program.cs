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

app.MapPut("/Transfer/Approve/{id}", async (int id, MoneyTransferContext context) =>
{
    if (id <= 0) {  return Results.BadRequest(); }
    if (context is null || context.Transfers is null) { return Results.StatusCode(500); }

    Transfer findTransfer = (await context.Transfers.Include(transfer => transfer.AccountIdFromNavigation)
        .SingleOrDefaultAsync(transfer => transfer.Id == id))!;
    
    if (findTransfer is null) { return Results.NotFound(); }
    if (!findTransfer.IsValidForApproveOrReject || 
        !AccountFromHasSufficientFundsForTransfer(findTransfer.Amount, 
            findTransfer.AccountIdFromNavigation.CurrentBalance(), context)) { return Results.BadRequest(); }

    findTransfer.TransferStatusId = (int)TransferStatus.Approved;
    await context.SaveChangesAsync();
    return Results.NoContent();
});

app.MapGet("/Transfer/Details/{id}", async (int id, MoneyTransferContext context) =>
{
    if (id <= 0) { return Results.BadRequest(); }
    if (context is null || context.Transfers is null) { return Results.StatusCode(500); }
    
    return await context.Transfers.Select(t =>
        new 
        {
            Id = t.Id,
            Amount = t.Amount,
            TransferStatus = t.TransferStatus.ToString(),
            TransferType = t.TransferType.ToString(),
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
        .SingleOrDefaultAsync(transfer => transfer.Id == id) is object transfer
            ? Results.Ok(transfer)
            : Results.NotFound();
});

app.MapPut("/Transfer/Reject/{id}", async (int id, MoneyTransferContext context) =>
{
    if (id <= 0) { return Results.BadRequest(); }
    if (context is null || context.Transfers is null) { return Results.StatusCode(500); }

    Transfer findTransfer = (await context.Transfers.Include(transfer => transfer.AccountIdFromNavigation)
        .SingleOrDefaultAsync(transfer => transfer.Id == id))!;

    if (findTransfer is null) { return Results.NotFound(); }
    if (!findTransfer.IsValidForApproveOrReject) { return Results.BadRequest(); }

    findTransfer.TransferStatusId = (int)TransferStatus.Rejected;
    await context.SaveChangesAsync();
    return Results.NoContent();
});

app.MapPost("/Transfer/Request", async (AddTransfer transfer, MoneyTransferContext context) =>
{
    if (transfer is null || !transfer.IsValid) { return Results.BadRequest(); }
    if (context is null || context.Accounts is null || context.Transfers is null) { return Results.StatusCode(500); }

    Account accountFrom = (await context.Accounts
        .Include(account => account.TransferAccountIdFromNavigations)
        .Include(account => account.TransferAccountIdToNavigations)
        .SingleOrDefaultAsync(account => account.User.Username == transfer.UserFromName))!;

    if (accountFrom is null) { return Results.BadRequest(); }

    Transfer transferToAdd = new()
    {
        Id = 0,
        TransferStatusId = (int)TransferStatus.Pending,
        TransferTypeId = (int)TransferType.Request,
        DateCreated = DateOnly.FromDateTime(DateTime.Today),
        Amount = transfer.Amount,
        AccountIdFrom = accountFrom.Id,
        AccountIdTo =
            await context.Accounts.SingleOrDefaultAsync
                (account => account.User.Username == transfer.UserToName) is Account accountTo
                ? accountTo.Id
                : 0,
    };

    if (!transferToAdd.IsValidForAdd) { return Results.BadRequest(); }

    context.Transfers.Add(transferToAdd);
    await context.SaveChangesAsync();
    return Results.Created($"/Transfer/Details/{transferToAdd.Id}", transferToAdd);
});

app.MapPost("/Transfer/Send", async (AddTransfer transfer, MoneyTransferContext context) =>
{
    if (transfer is null || !transfer.IsValid) { return Results.BadRequest(); }
    if (context is null || context.Accounts is null || context.Transfers is null) { return Results.StatusCode(500); }
    
    Account accountFrom = (await context.Accounts
        .Include(account => account.TransferAccountIdFromNavigations)
        .Include(account => account.TransferAccountIdToNavigations)
        .SingleOrDefaultAsync(account => account.User.Username == transfer.UserFromName))!;

    if (accountFrom is null ||
        !AccountFromHasSufficientFundsForTransfer(transfer.Amount, accountFrom.CurrentBalance(), context))
        { return Results.BadRequest(); }
        
    Transfer transferToAdd = new()
    {
        Id = 0,
        TransferStatusId = (int)TransferStatus.Approved,
        TransferTypeId = (int)TransferType.Send,
        DateCreated = DateOnly.FromDateTime(DateTime.Today),
        Amount = transfer.Amount,
        AccountIdFrom = accountFrom.Id,
        AccountIdTo =
            await context.Accounts.SingleOrDefaultAsync
                (account => account.User.Username == transfer.UserToName) is Account accountTo
                ? accountTo.Id
                : 0,
    };

    if (!transferToAdd.IsValidForAdd) { return Results.BadRequest(); }    
    
    context.Transfers.Add(transferToAdd);
    await context.SaveChangesAsync();
    return Results.Created($"/Transfer/Details/{transferToAdd.Id}", transferToAdd);
});

app.MapGet("/User/Account/Details/{id}", async (int id, MoneyTransferContext context) =>
    {
        if (id <= 0) { return Results.BadRequest(); }
        if (context is null || context.Accounts is null) { return Results.StatusCode(500); }

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
                    CurrentBalance = a.CurrentBalance(),
                    DateCreated = a.DateCreated,
                }).SingleOrDefaultAsync() is object account            
        ? Results.Ok(account)
        : Results.NotFound();
    });

app.MapGet("/User/Transfer/Completed/{id}", async (int id, MoneyTransferContext context) =>
{
    // TODO: Debug why enabling the next two (2) lines breaks the lambda expression
    //if (id <= 0) { return Results.BadRequest(); }
    //if (context is null || context.Transfers is null) { return Results.StatusCode(500); }

    return await context.Transfers.Where(transfer =>
        (transfer.AccountIdFromNavigation.UserId == id
          || transfer.AccountIdToNavigation.UserId == id)
          && transfer.TransferStatusId != (int)TransferStatus.Pending)
        .Select(t => new
        {
            Id = t.Id,
            Amount = t.Amount,
            TransferStatus = t.TransferStatus.ToString(),
            TransferType = t.TransferType.ToString(),
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
        //if (id <= 0) { return Results.BadRequest(); }
        //if (context is null || context.Accounts is null) { return Results.StatusCode(500); }

        return await context.Transfers.Where(transfer =>
            (transfer.AccountIdFromNavigation.UserId == id
              || transfer.AccountIdToNavigation.UserId == id)
              && transfer.TransferStatusId == (int)TransferStatus.Pending)
            .Select(t => new 
            {
                Id = t.Id,
                Amount = t.Amount,
                TransferStatus = t.TransferStatus.ToString(),
                TransferType = t.TransferType.ToString(),
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

app.Run();

static bool AccountFromHasSufficientFundsForTransfer
    (decimal amount, decimal accountFromCurrentBalance, MoneyTransferContext context) => 
        accountFromCurrentBalance > 0 && accountFromCurrentBalance >= amount;
