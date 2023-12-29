# MoneyTransfer

## An application that emulates some of the functionality of a very popular money transfer app

### Built with: 
- .NET 8 / C# 12
- SQL Server database
- API: ASP.NET Core, minimal API, Entity Framework Core 8
- Security: Class library for hashing passwords and generating JWTs
- UI: .NET MAUI, targeting windows desktop only
- Programming techniques used application-wide:
	- Asynchronous programming
	- Dependency injection
	- Null object pattern (first time I've used it)
	- Primary constructors (first time I've used them)
	- Immutable objects and collections (first time I've used them)
	- Anonymous objects (first time I've used them)

## Features:
- Register as a new user
- Log In as an existing user
- Log Out as a logged in user
- Get account details, including current balance
- View completed transfer requests
- View pending transfer requests
- View the details of a transfer
- Send a transfer to another user
- Request a transfer from another user
- Approve a transfer request
- Reject a transfer request

## Business rules:
- New users start with a balance of $1,000
- A user can request a transfer of any amount from another user
	- A requested transfer has 'pending' status, and can later be approved or rejected by the requestee
- A user can send a transfer of any amount equal to or less than their current balance to another user
	- A sent transfer request has 'approved' status
- A user can approve a transfer request from another user if the amount is equal to or less than their current balance
- A user can reject any transfer request from another user
- Only 'pending' transfer requests can be approved or rejected
- The user from and user to for a given transfer must be different users

## UI conventions:
- Users presented in lists are sorted by username
- Transfers presented in lists are sorted by date descending

## Instructions for running the application:
- Note that SQL server and Visual Studio are required for running the application
- Clone or download the repo
- Browse to \MoneyTransfer\Database
- Run the database script 'moneytransfer-create-db-and-initialize-data.sql'
	- This script will drop (if exists) and re-create the database and tables
	- The script inserts a small amount of data required by the application into the database 
	- Note that there is optional sample data that can be inserted into the database as well; it can be found at the end of the database script and it is commented out
- Browse to \MoneyTransfer\MoneyTransfer.API\appsettings.json
	- There is a database connection string in this config file that needs to point to your database
- Run the solution in Visual Studio

## Improvement opportunities:
- Additional UIs: other web and desktop UIs could easily be built on top of the API and security layers
- Register user UI needs better error reporting for invalid username and password inputs
- Log in user UI needs better error reporting for invalid username and password inputs, and incorrect password
- Review and revise the api for appropriate return types and status codes, appropriate use of validation, appropriate route names (nouns not verbs, e.g., post to '/Items' rather than '/Items/Create'), whether or not returning anonymous objects is a good idea and why...
- There is some xaml duplication in the MAUI project pages that have similar functionality (send transfer/request transfer, approve transfer request/reject transfer request, completed transfers/pending transfers); this could be remediated with content templates but this might also introduce complications with data bindings
- The http data service in the MAUI project seems a bit slow, look for performance improvements there
- Implement community toolkit validation: https://learn.microsoft.com/en-us/dotnet/architecture/maui/validation
- In the MAUI project, move the base url used in the http data service and the user service into configuration (appsettings.json)
- Implement password requirements