using Microsoft.EntityFrameworkCore;
using MoneyTransfer.API.DataAccess;

var builder = WebApplication.CreateBuilder(args);

var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
            .Build();

var connectionString = config.GetConnectionString("Project");

builder.Services.AddDbContext<MoneyTransferContext>(options => options.UseSqlServer(connectionString));

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapGet("/ApproveTransferRequest/{transferId}", ()=>
    (int transferId, MoneyTransferContext context) =>
        context.ApproveTransferRequest(transferId));

app.MapGet("/GetAccountDetailsForUser/{username}", () =>
    (string username, MoneyTransferContext context) =>
        context.GetAccountDetailsForUser(username));

app.MapGet("/GetCompletedTransfersForUser/{username}", () =>
    (string username, MoneyTransferContext context) =>
        context.GetCompletedTransfersForUser(username));

app.MapGet("/GetPendingTransfersForUser/{username}", () =>
    (string username, MoneyTransferContext context) =>
        context.GetPendingTransfersForUser(username));

app.MapGet("/GetTransferDetails/{transferId}", () =>
    (int transferId, MoneyTransferContext context) =>
        context.GetTransferDetails(transferId));

app.MapGet("/RejectTransferRequest/{transferId}", () =>
    (int transferId, MoneyTransferContext context) =>
        context.RejectTransferRequest(transferId));

app.MapGet("/RequestTransfer", () =>
    (string userFromName, string userToName, decimal amount, 
        MoneyTransferContext context) =>
            context.RequestTransfer(userFromName, userToName, amount));

app.MapGet("/SendTransfer", () =>
    (string userFromName, string userToName, decimal amount,
        MoneyTransferContext context) =>
            context.SendTransfer(userFromName, userToName, amount));

app.Run();
