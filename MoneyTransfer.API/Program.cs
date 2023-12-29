using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MoneyTransfer.API.Context;
using MoneyTransfer.API.Entities;
using MoneyTransfer.Security;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
            .Build();

string connectionString = config.GetConnectionString("Project") ?? "Error retrieving connection string!";
string jwtSecret = config.GetValue<string>("JwtSecret") ?? "Error retreiving jwt config!";

var key = Encoding.ASCII.GetBytes(jwtSecret);
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap[JwtRegisteredClaimNames.Sub] = "sub";

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        NameClaimType = "name"
    };
});

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("requireauthuser", policy => policy.RequireAuthenticatedUser());

builder.Services.AddDbContext<MoneyTransferContext>(options =>
    options.UseSqlServer(connectionString));
//.ConfigureHttpJsonOptions(options =>
//    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles)

builder.Services
    .AddSingleton<ITokenGenerator>(tk => new JwtGenerator(jwtSecret))
    .AddSingleton<IPasswordHasher>(ph => new PasswordHasher());

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapPut("/Transfer/Approve/{id}", async (int id, object transfer, MoneyTransferContext context) =>
{
    if (id <= 0) { return Results.BadRequest(); }
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
}).RequireAuthorization("requireauthuser");

app.MapGet("/Transfer/Details/{id}", async (int id, MoneyTransferContext context) =>
{
    return id <= 0
        ? Results.BadRequest()
        : context is null || context.Transfers is null
        ? Results.StatusCode(500)
        : await context.Transfers.Where(transfer => transfer.Id == id).Select(t =>
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
        }).Select(a =>
                new
                {
                    Id = a.Id,
                    DateCreated = a.DateCreated,
                    Amount = a.Amount,
                    TransferStatus = a.TransferStatus,
                    TransferType = a.TransferType,
                    UserFromName = a.AccountIdFromNavigation.User.Username,
                    UserToName = a.AccountIdToNavigation.User.Username,

                }).SingleOrDefaultAsync() is object transfer
        ? Results.Ok(transfer)
        : Results.NotFound();
}).RequireAuthorization("requireauthuser");

app.MapPut("/Transfer/Reject/{id}", async (int id, object transfer, MoneyTransferContext context) =>
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
}).RequireAuthorization("requireauthuser");

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

    await context.Transfers.AddAsync(transferToAdd);
    await context.SaveChangesAsync();
    return Results.Created($"/Transfer/Details/{transferToAdd.Id}", transferToAdd);
}).RequireAuthorization("requireauthuser");

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
}).RequireAuthorization("requireauthuser");

app.MapGet("/User/Account/Details/{id}", async (int id, MoneyTransferContext context) =>
    {
        return id <= 0
            ? Results.BadRequest()
            : context is null || context.Accounts is null
                ? Results.StatusCode(500)
                : await context.Accounts.Where(a => a.User.Id == id).Select(a =>
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
    }).RequireAuthorization("requireauthuser");

app.MapGet("/User/{id}", async (int id, MoneyTransferContext context) =>
{
    return id <= 0
        ? Results.BadRequest()
        : context is null || context.Users is null
        ? Results.StatusCode(500)
        : await context.Users
            .Where(user => user.Id == id)
            .Select(user => new ReturnUser()
            {
                Id = user.Id,
                Username = user.Username,
                Token = null!,
            })
            .Select(a => new
            {
                Id = a.Id,
                Username = a.Username,
                Token = a.Token,
                Message = string.Empty,
            })
            .SingleOrDefaultAsync() is object user
                ? Results.Ok(user)
                : Results.NotFound();
}).RequireAuthorization("requireauthuser");

app.MapGet("/User/GetUsers", async Task<object> (MoneyTransferContext context) =>
{
    return context is null || context.Users is null
        ? Results.StatusCode(500)
        : await context.Users
            .Select(user => new ReturnUser()
            {
                Id = user.Id,
                Username = user.Username,
                Token = null!,
            })
            .Select(a => new
            {
                Id = a.Id,
                Username = a.Username,
                Token = a.Token,
                Message = string.Empty,
            })
            .OrderBy(a => a.Username)
            .ToListAsync();
}).RequireAuthorization("requireauthuser");

app.MapPost("/User/LogIn", async (LogInUser logInUser, MoneyTransferContext context, IPasswordHasher passwordHasher, ITokenGenerator tokenGenerator) =>
{
    if (logInUser is null || !logInUser.IsValid()) { return Results.BadRequest(); }
    if (context is null || context.Users is null || passwordHasher is null || tokenGenerator is null)
    { return Results.StatusCode(500); }

    // Get the user by username
    User user = await context.Users.SingleOrDefaultAsync(user => user.Username == logInUser.Username) ?? User.NotFound;

    // If we found a user and the password hash matches
    if (user.Id > 0 && passwordHasher.VerifyHashMatch(user.PasswordHash, logInUser.Password, user.Salt))
    {
        // Create an authentication token
        string token = tokenGenerator.GenerateToken(user.Id, user.Username);

        // Create a ReturnUser object to return to the client
        ReturnUser retUser = new() { Id = user.Id, Username = user.Username, Token = token };

        return Results.Ok(retUser);
    }

    return Results.BadRequest();
});

app.MapPost("/User/Register", async (LogInUser registerUser, MoneyTransferContext context, IPasswordHasher passwordHasher) =>
{
    if (registerUser is null || !registerUser.IsValid()) { return Results.BadRequest(); }
    if (context is null || context.Users is null || passwordHasher is null)
    { return Results.StatusCode(500); }

    User existingUser = await context.Users.SingleOrDefaultAsync(user => user.Username == registerUser.Username) ?? User.NotFound;
    if (existingUser.Id > 0)
    {
        return Results.Conflict(new { message = "Username already taken. Please choose a different username." });
    }

    PasswordHash hash = passwordHasher.ComputeHash(registerUser.Password);

    User userToRegister = new()
    {
        Username = registerUser.Username,
        PasswordHash = hash.Password,
        Salt = hash.Salt,
        Account = new()
        {
            StartingBalance = 1000M,
            DateCreated = DateOnly.FromDateTime(DateTime.Today),
        },
    };

    context.Users.Add(userToRegister);
    await context.SaveChangesAsync();
    return Results.Created($"/User/{userToRegister.Id}", userToRegister);
});

app.MapGet("/User/Transfer/Completed/{id}", async Task<object> (int id, MoneyTransferContext context) =>
    {
        return id <= 0
            ? Results.BadRequest()
            : context is null || context.Transfers is null
                ? Results.StatusCode(500)
                : await context.Transfers.Where(transfer =>
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
                }).Select(a =>
                    new
                    {
                        Id = a.Id,
                        DateCreated = a.DateCreated,
                        Amount = a.Amount,
                        TransferStatus = a.TransferStatus,
                        TransferType = a.TransferType,
                        UserFromName = a.AccountIdFromNavigation.User.Username,
                        UserToName = a.AccountIdToNavigation.User.Username,
                    })
                    .OrderByDescending(a => a.DateCreated)
                    .ToListAsync();
    }).RequireAuthorization("requireauthuser");

app.MapGet("/User/Transfer/Pending/{id}", async Task<object> (int id, MoneyTransferContext context) =>
    {
        return id <= 0
            ? Results.BadRequest()
            : context is null || context.Transfers is null
                ? Results.StatusCode(500)
                : await context.Transfers.Where(transfer =>
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
                                Username = t.AccountIdFromNavigation.User.Username,
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
                    }).Select(a =>
                        new
                        {
                            Id = a.Id,
                            DateCreated = a.DateCreated,
                            Amount = a.Amount,
                            TransferStatus = a.TransferStatus,
                            TransferType = a.TransferType,
                            UserFromName = a.AccountIdFromNavigation.User.Username,
                            UserToName = a.AccountIdToNavigation.User.Username,
                        })
                        .OrderByDescending(a => a.DateCreated)
                        .ToListAsync();
    }).RequireAuthorization("requireauthuser");

app.Run();

static bool AccountFromHasSufficientFundsForTransfer(decimal amount, decimal accountFromCurrentBalance, MoneyTransferContext context)
{
    return accountFromCurrentBalance > 0 && accountFromCurrentBalance >= amount;
}