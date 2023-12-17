using Microsoft.EntityFrameworkCore;
using MoneyTransfer.API.Context;
using MoneyTransfer.API.Entities;

var builder = WebApplication.CreateBuilder(args);

var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
            .Build();

string connectionString = config.GetConnectionString("Project") ?? "Error retrieving connection string!";

builder.Services.AddDbContext<MoneyTransferContext>(options =>
    options.UseSqlServer(connectionString));

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapGet("/Account/Get", async (MoneyTransferContext context) =>
    await context.Accounts.ToListAsync());

app.MapGet("/Account/Get/{id}", async 
    (int id, MoneyTransferContext context) =>
        await context.Accounts.FindAsync(id) is Account account
            ? Results.Ok(account)
            : Results.NotFound());

app.MapGet("/Transfer/Get", async (MoneyTransferContext context) => 
    await context.Transfers.ToListAsync());

app.MapGet("/Transfer/Get/{id}", async
    (int id, MoneyTransferContext context) => 
        await context.Transfers.FindAsync(id) is Transfer transfer 
            ? Results.Ok(transfer) 
            : Results.NotFound());

app.MapPost("/Account/Create", async
    (Account account, MoneyTransferContext context) => {
        context.Accounts.Add(account);
        await context.SaveChangesAsync();

        return Results.Created($"/Account/Get/{account.Id}", account);
    });

app.MapPost("/Transfer/Create", async
    (Transfer transfer, MoneyTransferContext context) => {
        context.Transfers.Add(transfer);
        await context.SaveChangesAsync();

        return Results.Created($"/Transfer/Get/{transfer.Id}", transfer);
    });

app.MapPut("/Account/Update/{id}", async
    (int id, Account account, MoneyTransferContext context) => {
        Account findAccount = (await context.Accounts.FindAsync(id))!;

        if (findAccount is null) { return Results.NotFound(); }

        findAccount.Id = account.Id;
        findAccount.UserId = account.UserId;
        findAccount.User = account.User;
        findAccount.DateCreated = account.DateCreated;
        findAccount.StartingBalance = account.StartingBalance;
        findAccount.TransferAccountIdFromNavigations = account.TransferAccountIdFromNavigations;
        findAccount.TransferAccountIdToNavigations = account.TransferAccountIdToNavigations;

        await context.SaveChangesAsync();

        return Results.NoContent();
    });

app.MapPut("/Transfer/Update/{id}",
    async (int id, Transfer transfer, MoneyTransferContext context) => {
        Transfer findTransfer = (await context.Transfers.FindAsync(id))!;

        if (findTransfer is null) { return Results.NotFound(); }

        findTransfer.Id = transfer.Id;
        findTransfer.Amount = transfer.Amount;
        findTransfer.AccountIdFrom = transfer.AccountIdFrom;
        findTransfer.AccountIdTo = transfer.AccountIdTo;
        findTransfer.AccountIdFromNavigation = transfer.AccountIdFromNavigation;
        findTransfer.AccountIdToNavigation = transfer.AccountIdToNavigation;
        findTransfer.TransferStatusId = transfer.TransferStatusId;
        findTransfer.TransferTypeId = transfer.TransferTypeId;
        findTransfer.DateCreated = transfer.DateCreated;
        
        await context.SaveChangesAsync();

        return Results.NoContent();
    });

app.MapDelete("/Account/Delete/{id}", async
    (int id, MoneyTransferContext context) => {
        if (await context.Accounts.FindAsync(id) is Account account)
        {
            context.Accounts.Remove(account);
            await context.SaveChangesAsync();
            return Results.NoContent();
        }

        return Results.NotFound();
    });

app.MapDelete("/Transfer/Delete/{id}", async
    (int id, MoneyTransferContext context) => {
        if (await context.Transfers.FindAsync(id) is Transfer transfer)
        {
            context.Transfers.Remove(transfer);
            await context.SaveChangesAsync();
            return Results.NoContent();
        }

        return Results.NotFound();
    });


app.Run();
