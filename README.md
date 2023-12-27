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
- TBD

## UI conventions:
- TBD

## Instructions for running the application:
- TBD

## Improvement opportunities:
- Review and revise the api for appropriate return types and status codes, appropriate use of validation, appropriate route names (nouns not verbs, e.g., post to '/Items' rather than '/Items/Create'), whether or not returning anonymous objects is a good idea and why...
- There is some xaml duplication in the MAUI project pages that have similar functionality (send transfer/request transfer, approve transfer request/reject transfer request, completed transfers/pending transfers); this could be remediated with content templates but this might also introduce complications with data bindings
- The http data service in the MAUI project seems a bit slow, look for performance improvements there
- Implement community toolkit validation: https://learn.microsoft.com/en-us/dotnet/architecture/maui/validation
- In the MAUI project, move the base url used in the http data service and the user service into configuration (appsettings.json)

## What I learned from this project:
- TBD








