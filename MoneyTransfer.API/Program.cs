using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MoneyTransfer.Core.DTO;
using MoneyTransfer.Core.Entities;
using MoneyTransfer.Security;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Context = MoneyTransfer.API.Context.MoneyTransferContext;

var builder = WebApplication.CreateBuilder(args);

IConfigurationRoot config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
            .Build();

string connectionString = config.GetConnectionString("Project") ?? "Error retrieving connection string!";
string jwtSecret = config.GetValue<string>("JwtSecret") ?? "Error retreiving jwt config!";

byte[] key = Encoding.ASCII.GetBytes(jwtSecret);
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

builder.Services.AddDbContext<Context>(options =>
    options.UseSqlServer(connectionString));

builder.Services
    .AddSingleton<ITokenGenerator>(tk => new JwtGenerator(jwtSecret))
    .AddSingleton<IPasswordHasher>(ph => new PasswordHasher());

var app = builder.Build();

app.MapGet("/", () => "Welcome to Money Transfer!");

app.MapPut("/Transfer/Approve/{id:int}", async Task<Results<BadRequest<string>, NotFound<string>, NoContent>> (Context context, int id, TransferDetailsDTO dto) =>
{
    if (id < 1)
    {
        return TypedResults.BadRequest("Invalid transfer id.");
    }
    if (!dto.Validate().IsValid)
    {
        return TypedResults.BadRequest("The information provided with the request is invalid.");
    }
    if (!id.Equals(dto.Id))
    {
        return TypedResults.BadRequest($"Transfer id mismatch. The id provided with the http request is {id}, and the id provided with the request content is {dto.Id}.");
    }

    Transfer entity = (await context.Transfers.Include(t => t.AccountIdFromNavigation).SingleOrDefaultAsync(t => t.Id.Equals(id)))!;

    if (entity is null)
    {
        return TypedResults.NotFound($"Transfer id {id} not found.");
    }
    if (!entity.IsValidForApproveOrReject)
    {
        return TypedResults.BadRequest("Transfer id {id} cannot be approved, as it is not in Pending status.");
    }
    if (!AccountFromHasSufficientFundsForTransfer(entity.Amount, entity.AccountIdFromNavigation.CurrentBalance()))
    {
        return TypedResults.BadRequest("Insufficient funds to complete the transfer.");
    }

    entity.TransferStatusId = (int)TransferStatus.Approved;

    await context.SaveChangesAsync();

    return TypedResults.NoContent();

}).RequireAuthorization("requireauthuser");

app.MapPut("/Transfer/Reject/{id:int}", async Task<Results<BadRequest<string>, NotFound<string>, NoContent>> (Context context, int id, TransferDetailsDTO dto) =>
{
    if (id < 1)
    {
        return TypedResults.BadRequest("Invalid transfer id.");
    }
    if (!dto.Validate().IsValid)
    {
        return TypedResults.BadRequest("The information provided with the request is invalid.");
    }
    if (!id.Equals(dto.Id))
    {
        return TypedResults.BadRequest($"Transfer id mismatch. The id provided with the http request is {id}, and the id provided with the request content is {dto.Id}.");
    }

    Transfer entity = (await context.Transfers.SingleOrDefaultAsync(t => t.Id.Equals(id)))!;

    if (entity is null)
    {
        return TypedResults.NotFound($"Transfer id {id} not found.");
    }
    if (!entity.IsValidForApproveOrReject)
    {
        return TypedResults.BadRequest("Transfer id {id} cannot be rejected, as it is not in Pending status.");
    }

    entity.TransferStatusId = (int)TransferStatus.Rejected;

    await context.SaveChangesAsync();

    return TypedResults.NoContent();

}).RequireAuthorization("requireauthuser");

app.MapPost("/Transfer/Request", async Task<Results<BadRequest<string>, Created<TransferDetailsDTO>>> (Context context, AddTransferDTO dto) =>
{
    if (!dto.Validate().IsValid)
    {
        return TypedResults.BadRequest("The information provided with the request is invalid.");
    }

    Account accountFrom = ((await context.Accounts
        .SingleOrDefaultAsync(a => a.User.Username.Equals(dto.UserFromName))))!;

    if (accountFrom is null)
    {
        return TypedResults.BadRequest("Unable to find account to transfer from.");
    }

    Transfer transferToAdd = new()
    {
        TransferStatusId = (int)TransferStatus.Pending,
        TransferTypeId = (int)TransferType.Request,
        DateCreated = DateOnly.FromDateTime(DateTime.Today),
        Amount = dto.Amount,
        AccountIdFrom = accountFrom.Id,
        AccountIdTo =
            await context.Accounts.SingleOrDefaultAsync
                (a => a.User.Username.Equals(dto.UserToName)) is Account accountTo
                ? accountTo.Id
                : 0,
    };

    if (!transferToAdd.IsValidForAdd)
    {
        return TypedResults.BadRequest("The information provided with the request is invalid.");
    }

    await context.Transfers.AddAsync(transferToAdd);

    await context.SaveChangesAsync();

    TransferDetailsDTO newDto = new()
    {
        Id = transferToAdd.Id,
        Amount = transferToAdd.Amount,
        DateCreated = transferToAdd.DateCreated,
        UserFromName = dto.UserFromName,
        UserToName = dto.UserToName,
        TransferStatus = transferToAdd.TransferStatus.ToString(),
        TransferType = transferToAdd.TransferType.ToString()
    };

    return TypedResults.Created($"/Transfer/Details/{newDto.Id}", newDto);

}).RequireAuthorization("requireauthuser");

app.MapPost("/Transfer/Send", async Task<Results<BadRequest<string>, Created<TransferDetailsDTO>>> (Context context, AddTransferDTO dto) =>
{
    if (!dto.Validate().IsValid)
    {
        return TypedResults.BadRequest("The information provided with the request is invalid.");
    }

    Account accountFrom = ((await context.Accounts
        .SingleOrDefaultAsync(account => account.User.Username.Equals(dto.UserFromName))))!;

    if (accountFrom is null)
    {
        return TypedResults.BadRequest("Unable to find account to transfer from.");
    }

    Transfer transferToAdd = new()
    {
        TransferStatusId = (int)TransferStatus.Approved,
        TransferTypeId = (int)TransferType.Send,
        DateCreated = DateOnly.FromDateTime(DateTime.Today),
        Amount = dto.Amount,
        AccountIdFrom = accountFrom.Id,
        AccountIdTo =
            await context.Accounts.SingleOrDefaultAsync
                (a => a.User.Username.Equals(dto.UserToName)) is Account accountTo
                ? accountTo.Id
                : 0,
    };

    if (!transferToAdd.IsValidForAdd)
    {
        return TypedResults.BadRequest("The information provided with the request is invalid.");
    }

    await context.Transfers.AddAsync(transferToAdd);

    await context.SaveChangesAsync();

    TransferDetailsDTO newDto = new()
    {
        Id = transferToAdd.Id,
        Amount = transferToAdd.Amount,
        DateCreated = transferToAdd.DateCreated,
        UserFromName = dto.UserFromName,
        UserToName = dto.UserToName,
        TransferStatus = transferToAdd.TransferStatus.ToString(),
        TransferType = transferToAdd.TransferType.ToString()
    };

    return TypedResults.Created($"/Transfer/Details/{newDto.Id}", newDto);
}).RequireAuthorization("requireauthuser");

app.MapGet("/Transfer/Details/{id:int}", async Task<Results<BadRequest<string>, NotFound<string>, Ok<TransferDetailsDTO>>> (Context context, int id) =>
{
    if (id < 1)
    {
        return TypedResults.BadRequest("Invalid transfer id.");
    }

    Transfer entity = (await context.Transfers
        .Include(t => t.AccountIdToNavigation)
        .ThenInclude(a => a.User)
        .Include(t => t.AccountIdFromNavigation)
        .ThenInclude(a => a.User)
        .SingleOrDefaultAsync(t => t.Id.Equals(id)))!;

    if (entity is null)
    {
        return TypedResults.NotFound($"Transfer id {id} not found.");
    }

    TransferDetailsDTO dto = new()
    {
        Id = entity.Id,
        Amount = entity.Amount,
        DateCreated = entity.DateCreated,
        TransferStatus = entity.TransferStatus.ToString(),
        TransferType = entity.TransferType.ToString(),
        UserToName = entity.AccountIdToNavigation.User.Username,
        UserFromName = entity.AccountIdFromNavigation.User.Username,
    };

    return TypedResults.Ok(dto);

}).RequireAuthorization("requireauthuser");

app.MapPost("/User/Register", async Task<Results<BadRequest<string>, Conflict<string>, Created<UserDTO>>> (Context context, IPasswordHasher passwordHasher, LogInUserDTO registerUser) =>
{
    if (!registerUser.Validate().IsValid)
    {
        return TypedResults.BadRequest("The information provided with the request is invalid.");
    }

    User existingUser = await context.Users.SingleOrDefaultAsync(u => u.Username.Equals(registerUser.Username)) ?? User.NotFound;

    if (existingUser.Id > 0)
    {
        return TypedResults.Conflict("Username already taken. Please choose a different username.");
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

    UserDTO dto = new()
    {
        Id = userToRegister.Id,
        Username = userToRegister.Username,
        Token = string.Empty
    };

    return TypedResults.Created($"/User/{dto.Id}", dto);
});

app.MapPost("/User/LogIn", async Task<Results<BadRequest<string>, UnauthorizedHttpResult, Ok<UserDTO>>> (Context context, IPasswordHasher passwordHasher, ITokenGenerator tokenGenerator, LogInUserDTO logInUser) =>
{
    if (!logInUser.Validate().IsValid)
    {
        return TypedResults.BadRequest("The information provided with the request is invalid.");
    }

    // Get the user by username
    User user = await context.Users.SingleOrDefaultAsync(u => u.Username.Equals(logInUser.Username)) ?? User.NotFound;

    if (user is null || user.Id.Equals(0))
    {
        return TypedResults.Unauthorized();
    }

    if (!passwordHasher.VerifyHashMatch(user.PasswordHash, logInUser.Password, user.Salt))
    {
        return TypedResults.Unauthorized();
    }
    else  // If we found a user and the password hash matches
    {
        // Create an authentication token
        string token = tokenGenerator.GenerateToken(user.Id, user.Username);

        // Create a ReturnUser object to return to the client
        UserDTO dto = new()
        {
            Id = user.Id,
            Username = user.Username,
            Token = token,
        };

        return TypedResults.Ok(dto);
    }
});

app.MapGet("/User/Account/Details/{id:int}", async Task<Results<BadRequest<string>, NotFound<string>, Ok<AccountDetailsDTO>>> (Context context, int id) =>
{
    if (id < 1)
    {
        return TypedResults.BadRequest("Invalid user id.");
    }

    Account entity = (await context.Accounts
        .Include(a => a.User)
        .Include(a => a.TransferAccountIdFromNavigations)
        .Include(a => a.TransferAccountIdToNavigations)
        .SingleOrDefaultAsync(a => a.UserId.Equals(id)))!;

    if (entity is null)
    {
        return TypedResults.NotFound($"Account for user id {id} not found.");
    }

    AccountDetailsDTO dto = new()
    {
        Id = entity.Id,
        Username = entity.User.Username,
        DateCreated = entity.DateCreated,
        CurrentBalance = entity.CurrentBalance(),
    };

    return TypedResults.Ok(dto);

}).RequireAuthorization("requireauthuser");

app.MapGet("/User/{id:int}", async Task<Results<BadRequest<string>, NotFound<string>, Ok<UserDTO>>> (Context context, int id) =>
{
    if (id < 1)
    {
        return TypedResults.BadRequest("Invalid user id.");
    }

    User entity = (await context.Users.SingleOrDefaultAsync(u => u.Id.Equals(id)))!;

    if (entity is null)
    {
        return TypedResults.NotFound($"User id {id} not found.");
    }

    UserDTO dto = new()
    {
        Id = entity.Id,
        Username = entity.Username,
        Token = string.Empty,
    };

    return TypedResults.Ok(dto);

}).RequireAuthorization("requireauthuser");

app.MapGet("/User", Results<BadRequest<string>, NotFound<string>, Ok<IEnumerable<UserDTO>>> (Context context) =>
{
    IOrderedQueryable<User> entities = context.Users.OrderBy(u => u.Username);

    if (entities is null || !entities.Any())
    {
        return TypedResults.NotFound("No users found.");
    }

    List<UserDTO> dtoList = [];

    foreach (User entity in entities)
    {
        UserDTO dto = new()
        {
            Id = entity.Id,
            Username = entity.Username,
            Token = string.Empty,
        };

        dtoList.Add(dto);
    }

    return TypedResults.Ok(dtoList.AsEnumerable<UserDTO>());

}).RequireAuthorization("requireauthuser");

app.MapGet("/User/Transfer/Completed/{id:int}", Results<BadRequest<string>, NotFound<string>, Ok<IEnumerable<TransferDetailsDTO>>> (Context context, int id) =>
{
    if (id < 1)
    {
        return TypedResults.BadRequest("Invalid user id.");
    }

    IEnumerable<Transfer> entities = context.Transfers
        .Include(t => t.AccountIdToNavigation)
        .ThenInclude(a => a.User)
        .Include(t => t.AccountIdFromNavigation)
        .ThenInclude(a => a.User)
        .Where(t => (t.AccountIdFromNavigation.UserId.Equals(id) || t.AccountIdToNavigation.UserId.Equals(id))
            && !t.TransferStatusId.Equals((int)TransferStatus.Pending))
        .OrderByDescending(t => t.DateCreated)
        .AsEnumerable<Transfer>();

    if (entities is null)
    {
        return TypedResults.NotFound("No completed transfers found.");
    }

    List<TransferDetailsDTO> dtoList = [];

    foreach (Transfer entity in entities)
    {
        TransferDetailsDTO dto = new()
        {
            Id = entity.Id,
            DateCreated = entity.DateCreated,
            Amount = entity.Amount,
            TransferStatus = entity.TransferStatus.ToString(),
            TransferType = entity.TransferType.ToString(),
            UserToName = entity.AccountIdToNavigation.User.Username,
            UserFromName = entity.AccountIdFromNavigation.User.Username,
        };

        dtoList.Add(dto);
    }

    return TypedResults.Ok(dtoList.AsEnumerable<TransferDetailsDTO>());

}).RequireAuthorization("requireauthuser");

app.MapGet("/User/Transfer/Pending/{id}", Results<BadRequest<string>, NotFound<string>, Ok<IEnumerable<TransferDetailsDTO>>> (Context context, int id) =>
{
    if (id < 1)
    {
        return TypedResults.BadRequest("Invalid user id.");
    }

    IEnumerable<Transfer> entities = context.Transfers
        .Include(t => t.AccountIdToNavigation)
        .ThenInclude(a => a.User)
        .Include(t => t.AccountIdFromNavigation)
        .ThenInclude(a => a.User)
        .Where(t => (t.AccountIdFromNavigation.UserId.Equals(id) || t.AccountIdToNavigation.UserId.Equals(id))
            && t.TransferStatusId.Equals((int)TransferStatus.Pending))
        .OrderByDescending(t => t.DateCreated)
        .AsEnumerable<Transfer>();

    if (entities is null)
    {
        return TypedResults.NotFound("No pending transfers found.");
    }

    List<TransferDetailsDTO> dtoList = [];

    foreach (Transfer entity in entities)
    {
        TransferDetailsDTO dto = new()
        {
            Id = entity.Id,
            DateCreated = entity.DateCreated,
            Amount = entity.Amount,
            TransferStatus = entity.TransferStatus.ToString(),
            TransferType = entity.TransferType.ToString(),
            UserToName = entity.AccountIdToNavigation.User.Username,
            UserFromName = entity.AccountIdFromNavigation.User.Username,
        };

        dtoList.Add(dto);
    }

    return TypedResults.Ok(dtoList.AsEnumerable<TransferDetailsDTO>());

}).RequireAuthorization("requireauthuser");

app.Run();

static bool AccountFromHasSufficientFundsForTransfer(decimal amount, decimal accountFromCurrentBalance)
{
    return accountFromCurrentBalance > 0 && accountFromCurrentBalance >= amount;
}
