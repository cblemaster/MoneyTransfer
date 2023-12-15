using Microsoft.EntityFrameworkCore;
using MoneyTransfer.API.DataAccess;
using MoneyTransfer.API.Entities;

var builder = WebApplication.CreateBuilder(args);

var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json",
                optional: true)
            .Build();

string connectionString = config.GetConnectionString("Project") ?? "Error locating connection string";

builder.Services
    .AddTransient<ITransfersAndAccountsDAO>(d =>
        new TransfersAndAccountsSqlDAO(connectionString));

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapGet("/ApproveTransferRequest/{transferId}", async    
    (int transferId, ITransfersAndAccountsDAO dao) =>
        {
            if (transferId > 0)
            {
                await dao.ApproveTransferRequestAsync(transferId);
            }
            return Results.BadRequest();
        });

app.MapGet("/GetAccountDetailsForUser/{username}", async
    (string username, ITransfersAndAccountsDAO dao) =>
        {
            Account account = await dao.GetAccountDetailsForUserAsync(username);
            if (!account.IsValid()) { account = Account.NotFound; }
            return account;
        });

app.MapGet("/GetCompletedTransfersForUser/{username}", async
    (string username, ITransfersAndAccountsDAO dao) =>
        {
            List<Transfer> transfers = await dao.GetCompletedTransfersForUserAsync(username);
            if (!transfers.All(transfer => transfer.IsValid())) { transfers = new(); }
            return transfers;
        });

app.MapGet("/GetPendingTransfersForUser/{username}", async
    (string username, ITransfersAndAccountsDAO dao) =>
        {
            List<Transfer> transfers = await dao.GetPendingTransfersForUserAsync(username);
            if (!transfers.All(transfer => transfer.IsValid())) { transfers = new(); }
            return transfers;
        });

app.MapGet("/GetTransferDetails/{transferId}", async
    (int transferId, ITransfersAndAccountsDAO dao) =>
        { 
            Transfer transfer = await dao.GetTransferDetailsAsync(transferId);
            if (!transfer.IsValid()) { transfer = Transfer.NotFound; }
            return transfer;
        });

app.MapGet("/RejectTransferRequest/{transferId}", async
    (int transferId, ITransfersAndAccountsDAO dao) =>
        {
            if (transferId > 0)
            {
                await dao.RejectTransferRequestAsync(transferId);
            }
            return Results.BadRequest();
        });

app.MapGet("/RequestTransfer/{userFromName}/{userToName}/{amount}", async
    (string userFromName, string userToName, decimal amount,
        ITransfersAndAccountsDAO dao) =>
            {
                AddTransfer transfer = new()
                {
                    UserFromName = userFromName,
                    UserToName = userToName,
                    Amount = amount,
                };

                if (transfer.IsValid())
                {
                    await dao.RequestTransferAsync(userFromName, userToName, amount);
                }
                return Results.BadRequest();
            });

app.MapGet("/SendTransfer/{userFromName}/{userToName}/{amount}", async
    (string userFromName, string userToName, decimal amount,
        ITransfersAndAccountsDAO dao) =>
            {
                AddTransfer transfer = new()
                {
                    UserFromName = userFromName,
                    UserToName = userToName,
                    Amount = amount,
                };

                if (transfer.IsValid())
                {
                    await dao.SendTransferAsync(userFromName, userToName, amount);
                }
                return Results.BadRequest();
            });

app.Run();
