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
	- Primary constructors and immutable objects

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
- TBD

## What I learned from this project:
When I started this project I had been thinking about how ORMs make it easy for us to bring back or send way more data than we need to: an object representing all of the columns of a table row to update when only one field has changed, an object representing all of the columns of a table row to add when some of the properties have pre-defined values for newly added records, an object representing all of the columns of a table row to then just display one or two of those fields.

For the record, I don’t think this is a shortcoming of ORMs necessarily, but rather a situation where the technology makes it easy for developers to be lazy or thoughtless about the data their apps consume, since the ORM makes abstract so many of these concerns.

With this in mind, I started development at the database level, creating stored procedures that return only the data needed for a particular view, or that perform an insert or update. A lot of what could be considered application logic ended up in these procedures, which was just me being self-indulgent as I really enjoy playing around in databases.

But as I moved forward with the project, I started to see a big problem with this approach: it makes the application not very extensible.

Adding new features might not be so bad: add a new procedure in the db, add a new data access method to call this procedure, add a new API endpoint to call the data access method, update the UI.

But modifying existing features would be complex in that database procedures might have to be modified (potentially breaking them), data access methods might have to be modified (potentially breaking them),  and API endpoints might have to be modified (potentially breaking them).

Knowing what I know now…I'd be more inclined to start with an EF Core db context with table-mapped Db Sets for entities. Then I would have models that encapsulate only the data needed for a feature, and that use immutable objects as much as possible. These "targeted" models I would think belong in the client/UI layer. The backend models could and probably should represent all table columns and have both getters and setters.




