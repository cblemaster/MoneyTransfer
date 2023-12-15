using Microsoft.EntityFrameworkCore;
using MoneyTransfer.API.DataAccess;

var builder = WebApplication.CreateBuilder(args);

var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json",
                optional: true)
            .Build();

string connectionString = config.GetConnectionString("Project") ?? string.Empty;

builder.Services
    .AddTransient<ITransfersAndAccountsDAO>(d =>
        new TransfersAndAccountsSqlDAO(connectionString));

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapGet("/ApproveTransferRequest/{transferId}", async
    (int transferId, ITransfersAndAccountsDAO dao) =>
        await dao.ApproveTransferRequestAsync(transferId));

app.MapGet("/GetAccountDetailsForUser/{username}", async
    (string username, ITransfersAndAccountsDAO dao) =>
        await dao.GetAccountDetailsForUserAsync(username));

app.MapGet("/GetCompletedTransfersForUser/{username}", async
    (string username, ITransfersAndAccountsDAO dao) =>
        await dao.GetCompletedTransfersForUserAsync(username));

app.MapGet("/GetPendingTransfersForUser/{username}", async
    (string username, ITransfersAndAccountsDAO dao) =>
        await dao.GetPendingTransfersForUserAsync(username));

app.MapGet("/GetTransferDetails/{transferId}", async
    (int transferId, ITransfersAndAccountsDAO dao) =>
        await dao.GetTransferDetailsAsync(transferId));

app.MapGet("/RejectTransferRequest/{transferId}", async
    (int transferId, ITransfersAndAccountsDAO dao) =>
        await dao.RejectTransferRequestAsync(transferId));

app.MapGet("/RequestTransfer/{userFromName}/{userToName}/{amount}", async
    (string userFromName, string userToName, decimal amount,
        ITransfersAndAccountsDAO dao) =>
            await dao.RequestTransferAsync(userFromName, userToName, amount));

app.MapGet("/SendTransfer/{userFromName}/{userToName}/{amount}", async
    (string userFromName, string userToName, decimal amount,
        ITransfersAndAccountsDAO dao) =>
            await dao.SendTransferAsync(userFromName, userToName, amount));

app.Run();
