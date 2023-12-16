USE master
GO


-- drop and re-create database
DECLARE @SQL nvarchar(1000);
IF EXISTS (SELECT 1 FROM sys.databases WHERE name = N'MoneyTransfer')
BEGIN
    SET @SQL = N'USE MoneyTransfer;

                 ALTER DATABASE MoneyTransfer SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                 USE master;

                 DROP DATABASE MoneyTransfer;';
    EXEC (@SQL);
END;

CREATE DATABASE MoneyTransfer
GO

USE MoneyTransfer
GO

-- create tables
CREATE TABLE TransferTypes (
	Id					int IDENTITY(1,1)					NOT NULL,
	TransferType		varchar(10)							NOT NULL,
	CONSTRAINT PK_TransferTypes PRIMARY KEY (Id),
)
GO

CREATE TABLE TransferStatuses (
	Id					int IDENTITY(1,1)					NOT NULL,
	TransferStatus		varchar(10)							NOT NULL,
	CONSTRAINT PK_TransferStatuses PRIMARY KEY (Id),
)
GO

CREATE TABLE Users (
	Id					int IDENTITY(1,1)					NOT NULL,
	Username			varchar(50)							NOT NULL,
	PasswordHash		varchar(200)						NOT NULL,
	Salt				varchar(200)						NOT NULL,
	CONSTRAINT PK_Users PRIMARY KEY (Id),
	CONSTRAINT UC_Username UNIQUE (Username),
)
GO

CREATE TABLE Accounts (
	Id					int IDENTITY(1,1)					NOT NULL,
	UserId				int									NOT NULL,
	StartingBalance		decimal(13, 2)						NOT NULL,
	DateCreated			date								NOT NULL,	
	CONSTRAINT PK_Accounts PRIMARY KEY (Id),
	CONSTRAINT UC_UserId UNIQUE (UserId),
	CONSTRAINT FK_Accounts_Users FOREIGN KEY (UserId) REFERENCES Users (Id),
)
GO

CREATE TABLE Transfers (
	Id					int IDENTITY(1,1)					NOT NULL,
	TransferTypeId		int									NOT NULL,
	TransferStatusId	int									NOT NULL,
	AccountIdFrom		int									NOT NULL,
	AccountIdTo			int									NOT NULL,
	Amount				decimal(13, 2)						NOT NULL,
	DateCreated			date								NOT NULL,
	CONSTRAINT PK_Transfers PRIMARY KEY (Id),
	CONSTRAINT FK_Transfers_AccountsFrom FOREIGN KEY (AccountIdFrom) REFERENCES Accounts (Id),
	CONSTRAINT FK_Transfers_AccountsTo FOREIGN KEY (AccountIdTo) REFERENCES Accounts (Id),
	CONSTRAINT FK_Transfers_TransferStatuses FOREIGN KEY (TransferStatusId) REFERENCES TransferStatuses (Id),
	CONSTRAINT FK_Transfers_TransferTypes FOREIGN KEY (TransferTypeId) REFERENCES TransferTypes (Id),
	CONSTRAINT CK_Transfers_not_same_account CHECK  ((AccountIdFrom <> AccountIdTo)),
	CONSTRAINT CK_transfers_amount_gt_0 CHECK ((amount > 0)),
)
GO

-- create stored procedures
--CREATE PROCEDURE ApproveTransferRequest @transferId int
--AS
--	SET NOCOUNT ON;
--	UPDATE Transfers SET TransferStatusId = (SELECT ts.Id FROM TransferStatuses ts WHERE ts.TransferStatus = 'Approved') WHERE Id = @transferId;
--GO

--CREATE PROCEDURE GetAccountDetailsForUser @username varchar(50)
--AS
--	SET NOCOUNT ON;
--	DECLARE @start_balance decimal;
--	SET @start_balance = (SELECT a.StartingBalance FROM Accounts a WHERE a.UserId = (SELECT u.Id FROM Users u WHERE u.Username = @username));

--	DECLARE @total_to decimal;
--	SET @total_to = (SELECT SUM(t.Amount) FROM Transfers t INNER JOIN Accounts a ON (t.AccountIdTo = a.Id) INNER JOIN Users u ON (a.UserId = u.Id) WHERE u.Username = @username AND t.TransferStatusId = (SELECT ts.Id FROM TransferStatuses ts WHERE ts.TransferStatus = 'Approved'));

--	DECLARE @total_from decimal;
--	SET @total_from = (SELECT SUM(t.Amount) FROM Transfers t INNER JOIN Accounts a ON (t.AccountIdFrom = a.Id) INNER JOIN Users u ON (a.UserId = u.Id) WHERE u.Username = 'brian' AND t.TransferStatusId = (SELECT ts.Id FROM TransferStatuses ts WHERE ts.TransferStatus = 'Approved'));
	
--	SELECT a.Id, u.Username, (@start_balance + @total_to - @total_from) AS 'Current Balance'
--	FROM Accounts a
--	INNER JOIN Users u ON (a.UserId = u.Id)
--	WHERE u.Username = @username;
--GO

--CREATE PROCEDURE GetCompletedTransfersForUser @username varchar(50)
--AS
--	SET NOCOUNT ON;
--	SELECT t.Id AS 'Transfer Id', t.DateCreated AS 'Transfer Date', t.Amount AS 'Transfer Amount', ts.TransferStatus AS 'Transfer Status', tt.TransferType AS 'Transfer Type', uTo.Username AS 'User To', uFrom.Username AS 'User From'
--	FROM Transfers t
--	INNER JOIN Accounts aTo ON (t.AccountIdTo = aTo.Id)
--	INNER JOIN Accounts aFrom ON (t.AccountIdFrom = aFrom.Id)
--	INNER JOIN Users uTo ON (aTo.UserId = uTo.Id)
--	INNER JOIN Users uFrom ON (aFrom.UserId = uFrom.Id)
--	INNER JOIN TransferStatuses ts ON (t.TransferStatusId = ts.Id)
--	INNER JOIN TransferTypes tt ON (t.TransferTypeId = tt.Id)
--	WHERE t.TransferStatusId <> (SELECT ts.Id FROM TransferStatuses ts WHERE ts.TransferStatus = 'Pending')
--	AND (uTo.Username = @username OR uFrom.Username = @username);
--GO

--CREATE PROCEDURE GetPendingTransfersForUser @username varchar(50)
--AS
--	SET NOCOUNT ON;
--	SELECT t.Id AS 'Transfer Id', t.DateCreated AS 'Transfer Date', t.Amount AS 'Transfer Amount', ts.TransferStatus AS 'Transfer Status', tt.TransferType AS 'Transfer Type', uTo.Username AS 'User To', uFrom.Username AS 'User From'
--	FROM Transfers t
--	INNER JOIN Accounts aTo ON (t.AccountIdTo = aTo.Id)
--	INNER JOIN Accounts aFrom ON (t.AccountIdFrom = aFrom.Id)
--	INNER JOIN Users uTo ON (aTo.UserId = uTo.Id)
--	INNER JOIN Users uFrom ON (aFrom.UserId = uFrom.Id)
--	INNER JOIN TransferStatuses ts ON (t.TransferStatusId = ts.Id)
--	INNER JOIN TransferTypes tt ON (t.TransferTypeId = tt.Id)
--	WHERE t.TransferStatusId = (SELECT ts.Id FROM TransferStatuses ts WHERE ts.TransferStatus = 'Pending')
--	AND (uTo.Username = @username OR uFrom.Username = @username);
--GO

--CREATE PROCEDURE GetTransferDetails @transferId int
--AS
--	SET NOCOUNT ON;
--	SELECT t.Id AS 'Transfer Id', t.DateCreated AS 'Transfer Date', t.Amount AS 'Transfer Amount', ts.TransferStatus AS 'Transfer Status', tt.TransferType AS 'Transfer Type', uTo.Username AS 'User To', uFrom.Username AS 'User From'
--	FROM Transfers t
--	INNER JOIN Accounts aTo ON (t.AccountIdTo = aTo.Id)
--	INNER JOIN Accounts aFrom ON (t.AccountIdFrom = aFrom.Id)
--	INNER JOIN Users uTo ON (aTo.UserId = uTo.Id)
--	INNER JOIN Users uFrom ON (aFrom.UserId = uFrom.Id)
--	INNER JOIN TransferStatuses ts ON (t.TransferStatusId = ts.Id)
--	INNER JOIN TransferTypes tt ON (t.TransferTypeId = tt.Id)
--	WHERE t.Id = @transferId;
--GO

--CREATE PROCEDURE RejectTransferRequest @transferId int
--AS
--	SET NOCOUNT ON;
--	UPDATE Transfers SET TransferStatusId = (SELECT ts.Id FROM TransferStatuses ts WHERE ts.TransferStatus = 'Rejected') WHERE Id = @transferId;
--GO

--CREATE PROCEDURE RequestTransfer @userFromName varchar(50), @userToName varchar(50), @amount decimal
--AS
--	SET NOCOUNT ON;
--	INSERT INTO Transfers (AccountIdFrom, AccountIdTo, Amount, TransferStatusId, TransferTypeId, DateCreated) 
--	VALUES ((SELECT a.Id FROM Accounts a INNER JOIN Users u ON (a.UserId = u.Id) WHERE u.Username = @userFromName),
--		    (SELECT a.Id FROM Accounts a INNER JOIN Users u ON (a.UserId = u.Id) WHERE u.Username = @userToName),
--		    @amount,
--		    (SELECT ts.Id FROM TransferStatuses ts WHERE ts.TransferStatus = 'Pending'), 
--		    (SELECT tt.Id FROM TransferTypes tt WHERE tt.TransferType = 'Request'),
--			GETDATE()
--		   ); 
--GO

--CREATE PROCEDURE SendTransfer @userFromName varchar(50), @userToName varchar(50), @amount decimal
--AS
--	SET NOCOUNT ON;
--	INSERT INTO Transfers (AccountIdFrom, AccountIdTo, Amount, TransferStatusId, TransferTypeId, DateCreated) 
--	VALUES ((SELECT a.Id FROM Accounts a INNER JOIN Users u ON (a.UserId = u.Id) WHERE u.Username = @userFromName),
--			(SELECT a.Id FROM Accounts a INNER JOIN Users u ON (a.UserId = u.Id) WHERE u.Username = @userToName),
--			@amount,
--			(SELECT ts.Id FROM TransferStatuses ts WHERE ts.TransferStatus = 'Approved'), 
--			(SELECT tt.Id FROM TransferTypes tt WHERE tt.TransferType = 'Send'),
--			GETDATE()
--		   );
--GO

-- data required for application
INSERT INTO TransferStatuses (TransferStatus) VALUES ('Pending');
INSERT INTO TransferStatuses (TransferStatus) VALUES ('Approved');
INSERT INTO TransferStatuses (TransferStatus) VALUES ('Rejected');

INSERT INTO TransferTypes (TransferType) VALUES ('Request');
INSERT INTO TransferTypes (TransferType) VALUES ('Send');

-- optional sample data
--INSERT INTO Users (Username, PasswordHash, Salt) VALUES ('brian','placeholder','placeholder');
--INSERT INTO Users (Username, PasswordHash, Salt) VALUES ('oscar','placeholder','placeholder');
--INSERT INTO Users (Username, PasswordHash, Salt) VALUES ('george','placeholder','placeholder');
--INSERT INTO Users (Username, PasswordHash, Salt) VALUES ('bernice','placeholder','placeholder');

--INSERT INTO Accounts (UserId, StartingBalance, DateCreated) VALUES ((SELECT u.Id FROM Users u WHERE u.Username = 'bernice'), 1000, GETDATE());
--INSERT INTO Accounts (UserId, StartingBalance, DateCreated) VALUES ((SELECT u.Id FROM Users u WHERE u.Username = 'brian'), 1000, GETDATE());
--INSERT INTO Accounts (UserId, StartingBalance, DateCreated) VALUES ((SELECT u.Id FROM Users u WHERE u.Username = 'oscar'), 1000, GETDATE());
--INSERT INTO Accounts (UserId, StartingBalance, DateCreated) VALUES ((SELECT u.Id FROM Users u WHERE u.Username = 'george'), 1000, GETDATE());

--INSERT INTO Transfers (TransferStatusId, TransferTypeId, AccountIdFrom, AccountIdTo, Amount, DateCreated) VALUES ((SELECT ts.Id FROM TransferStatuses ts WHERE ts.TransferStatus = 'Pending'), (SELECT tt.Id FROM TransferTypes tt WHERE tt.TransferType = 'Send'), (SELECT a.Id FROM Accounts a INNER JOIN Users u ON (a.UserId = u.Id) WHERE u.Username = 'brian'), (SELECT a.Id FROM Accounts a INNER JOIN Users u ON (a.UserId = u.Id) WHERE u.Username = 'oscar'), 20, GETDATE());
--INSERT INTO Transfers (TransferStatusId, TransferTypeId, AccountIdFrom, AccountIdTo, Amount, DateCreated) VALUES ((SELECT ts.Id FROM TransferStatuses ts WHERE ts.TransferStatus = 'Approved'), (SELECT tt.Id FROM TransferTypes tt WHERE tt.TransferType = 'Request'), (SELECT a.Id FROM Accounts a INNER JOIN Users u ON (a.UserId = u.Id) WHERE u.Username = 'bernice'), (SELECT a.Id FROM Accounts a INNER JOIN Users u ON (a.UserId = u.Id) WHERE u.Username = 'george'), 11, GETDATE());
--INSERT INTO Transfers (TransferStatusId, TransferTypeId, AccountIdFrom, AccountIdTo, Amount, DateCreated) VALUES ((SELECT ts.Id FROM TransferStatuses ts WHERE ts.TransferStatus = 'Rejected'), (SELECT tt.Id FROM TransferTypes tt WHERE tt.TransferType = 'Send'), (SELECT a.Id FROM Accounts a INNER JOIN Users u ON (a.UserId = u.Id) WHERE u.Username = 'oscar'), (SELECT a.Id FROM Accounts a INNER JOIN Users u ON (a.UserId = u.Id) WHERE u.Username = 'george'), 7, GETDATE());
--INSERT INTO Transfers (TransferStatusId, TransferTypeId, AccountIdFrom, AccountIdTo, Amount, DateCreated) VALUES ((SELECT ts.Id FROM TransferStatuses ts WHERE ts.TransferStatus = 'Pending'), (SELECT tt.Id FROM TransferTypes tt WHERE tt.TransferType = 'Send'), (SELECT a.Id FROM Accounts a INNER JOIN Users u ON (a.UserId = u.Id) WHERE u.Username = 'george'), (SELECT a.Id FROM Accounts a INNER JOIN Users u ON (a.UserId = u.Id) WHERE u.Username = 'brian'), 12, GETDATE());
--INSERT INTO Transfers (TransferStatusId, TransferTypeId, AccountIdFrom, AccountIdTo, Amount, DateCreated) VALUES ((SELECT ts.Id FROM TransferStatuses ts WHERE ts.TransferStatus = 'Approved'), (SELECT tt.Id FROM TransferTypes tt WHERE tt.TransferType = 'Send'), (SELECT a.Id FROM Accounts a INNER JOIN Users u ON (a.UserId = u.Id) WHERE u.Username = 'brian'), (SELECT a.Id FROM Accounts a INNER JOIN Users u ON (a.UserId = u.Id) WHERE u.Username = 'george'), 16, GETDATE());
--INSERT INTO Transfers (TransferStatusId, TransferTypeId, AccountIdFrom, AccountIdTo, Amount, DateCreated) VALUES ((SELECT ts.Id FROM TransferStatuses ts WHERE ts.TransferStatus = 'Approved'), (SELECT tt.Id FROM TransferTypes tt WHERE tt.TransferType = 'Request'), (SELECT a.Id FROM Accounts a INNER JOIN Users u ON (a.UserId = u.Id) WHERE u.Username = 'bernice'), (SELECT a.Id FROM Accounts a INNER JOIN Users u ON (a.UserId = u.Id) WHERE u.Username = 'oscar'), 14, GETDATE());
--INSERT INTO Transfers (TransferStatusId, TransferTypeId, AccountIdFrom, AccountIdTo, Amount, DateCreated) VALUES ((SELECT ts.Id FROM TransferStatuses ts WHERE ts.TransferStatus = 'Rejected'), (SELECT tt.Id FROM TransferTypes tt WHERE tt.TransferType = 'Send'), (SELECT a.Id FROM Accounts a INNER JOIN Users u ON (a.UserId = u.Id) WHERE u.Username = 'oscar'), (SELECT a.Id FROM Accounts a INNER JOIN Users u ON (a.UserId = u.Id) WHERE u.Username = 'bernice'), 2, GETDATE());
--INSERT INTO Transfers (TransferStatusId, TransferTypeId, AccountIdFrom, AccountIdTo, Amount, DateCreated) VALUES ((SELECT ts.Id FROM TransferStatuses ts WHERE ts.TransferStatus = 'Rejected'), (SELECT tt.Id FROM TransferTypes tt WHERE tt.TransferType = 'Request'), (SELECT a.Id FROM Accounts a INNER JOIN Users u ON (a.UserId = u.Id) WHERE u.Username = 'george'), (SELECT a.Id FROM Accounts a INNER JOIN Users u ON (a.UserId = u.Id) WHERE u.Username = 'oscar'), 1, GETDATE());
--INSERT INTO Transfers (TransferStatusId, TransferTypeId, AccountIdFrom, AccountIdTo, Amount, DateCreated) VALUES ((SELECT ts.Id FROM TransferStatuses ts WHERE ts.TransferStatus = 'Rejected'), (SELECT tt.Id FROM TransferTypes tt WHERE tt.TransferType = 'Send'), (SELECT a.Id FROM Accounts a INNER JOIN Users u ON (a.UserId = u.Id) WHERE u.Username = 'brian'), (SELECT a.Id FROM Accounts a INNER JOIN Users u ON (a.UserId = u.Id) WHERE u.Username = 'bernice'), 23, GETDATE());
--INSERT INTO Transfers (TransferStatusId, TransferTypeId, AccountIdFrom, AccountIdTo, Amount, DateCreated) VALUES ((SELECT ts.Id FROM TransferStatuses ts WHERE ts.TransferStatus = 'Pending'), (SELECT tt.Id FROM TransferTypes tt WHERE tt.TransferType = 'Request'), (SELECT a.Id FROM Accounts a INNER JOIN Users u ON (a.UserId = u.Id) WHERE u.Username = 'bernice'), (SELECT a.Id FROM Accounts a INNER JOIN Users u ON (a.UserId = u.Id) WHERE u.Username = 'brian'), 15, GETDATE());
--INSERT INTO Transfers (TransferStatusId, TransferTypeId, AccountIdFrom, AccountIdTo, Amount, DateCreated) VALUES ((SELECT ts.Id FROM TransferStatuses ts WHERE ts.TransferStatus = 'Rejected'), (SELECT tt.Id FROM TransferTypes tt WHERE tt.TransferType = 'Send'), (SELECT a.Id FROM Accounts a INNER JOIN Users u ON (a.UserId = u.Id) WHERE u.Username = 'oscar'), (SELECT a.Id FROM Accounts a INNER JOIN Users u ON (a.UserId = u.Id) WHERE u.Username = 'bernice'), 5, GETDATE());
--INSERT INTO Transfers (TransferStatusId, TransferTypeId, AccountIdFrom, AccountIdTo, Amount, DateCreated) VALUES ((SELECT ts.Id FROM TransferStatuses ts WHERE ts.TransferStatus = 'Approved'), (SELECT tt.Id FROM TransferTypes tt WHERE tt.TransferType = 'Request'), (SELECT a.Id FROM Accounts a INNER JOIN Users u ON (a.UserId = u.Id) WHERE u.Username = 'george'), (SELECT a.Id FROM Accounts a INNER JOIN Users u ON (a.UserId = u.Id) WHERE u.Username = 'oscar'), 8, GETDATE());
--INSERT INTO Transfers (TransferStatusId, TransferTypeId, AccountIdFrom, AccountIdTo, Amount, DateCreated) VALUES ((SELECT ts.Id FROM TransferStatuses ts WHERE ts.TransferStatus = 'Rejected'), (SELECT tt.Id FROM TransferTypes tt WHERE tt.TransferType = 'Request'), (SELECT a.Id FROM Accounts a INNER JOIN Users u ON (a.UserId = u.Id) WHERE u.Username = 'brian'), (SELECT a.Id FROM Accounts a INNER JOIN Users u ON (a.UserId = u.Id) WHERE u.Username = 'bernice'), 12, GETDATE());
--INSERT INTO Transfers (TransferStatusId, TransferTypeId, AccountIdFrom, AccountIdTo, Amount, DateCreated) VALUES ((SELECT ts.Id FROM TransferStatuses ts WHERE ts.TransferStatus = 'Pending'), (SELECT tt.Id FROM TransferTypes tt WHERE tt.TransferType = 'Request'), (SELECT a.Id FROM Accounts a INNER JOIN Users u ON (a.UserId = u.Id) WHERE u.Username = 'bernice'), (SELECT a.Id FROM Accounts a INNER JOIN Users u ON (a.UserId = u.Id) WHERE u.Username = 'brian'), 21, GETDATE());
--INSERT INTO Transfers (TransferStatusId, TransferTypeId, AccountIdFrom, AccountIdTo, Amount, DateCreated) VALUES ((SELECT ts.Id FROM TransferStatuses ts WHERE ts.TransferStatus = 'Approved'), (SELECT tt.Id FROM TransferTypes tt WHERE tt.TransferType = 'Send'), (SELECT a.Id FROM Accounts a INNER JOIN Users u ON (a.UserId = u.Id) WHERE u.Username = 'oscar'), (SELECT a.Id FROM Accounts a INNER JOIN Users u ON (a.UserId = u.Id) WHERE u.Username = 'brian'), 23, GETDATE());
--INSERT INTO Transfers (TransferStatusId, TransferTypeId, AccountIdFrom, AccountIdTo, Amount, DateCreated) VALUES ((SELECT ts.Id FROM TransferStatuses ts WHERE ts.TransferStatus = 'Rejected'), (SELECT tt.Id FROM TransferTypes tt WHERE tt.TransferType = 'Request'), (SELECT a.Id FROM Accounts a INNER JOIN Users u ON (a.UserId = u.Id) WHERE u.Username = 'george'), (SELECT a.Id FROM Accounts a INNER JOIN Users u ON (a.UserId = u.Id) WHERE u.Username = 'bernice'), 3, GETDATE());
--INSERT INTO Transfers (TransferStatusId, TransferTypeId, AccountIdFrom, AccountIdTo, Amount, DateCreated) VALUES ((SELECT ts.Id FROM TransferStatuses ts WHERE ts.TransferStatus = 'Pending'), (SELECT tt.Id FROM TransferTypes tt WHERE tt.TransferType = 'Send'), (SELECT a.Id FROM Accounts a INNER JOIN Users u ON (a.UserId = u.Id) WHERE u.Username = 'brian'), (SELECT a.Id FROM Accounts a INNER JOIN Users u ON (a.UserId = u.Id) WHERE u.Username = 'oscar'), 7, GETDATE());
--INSERT INTO Transfers (TransferStatusId, TransferTypeId, AccountIdFrom, AccountIdTo, Amount, DateCreated) VALUES ((SELECT ts.Id FROM TransferStatuses ts WHERE ts.TransferStatus = 'Approved'), (SELECT tt.Id FROM TransferTypes tt WHERE tt.TransferType = 'Request'), (SELECT a.Id FROM Accounts a INNER JOIN Users u ON (a.UserId = u.Id) WHERE u.Username = 'george'), (SELECT a.Id FROM Accounts a INNER JOIN Users u ON (a.UserId = u.Id) WHERE u.Username = 'bernice'), 18, GETDATE());
--INSERT INTO Transfers (TransferStatusId, TransferTypeId, AccountIdFrom, AccountIdTo, Amount, DateCreated) VALUES ((SELECT ts.Id FROM TransferStatuses ts WHERE ts.TransferStatus = 'Approved'), (SELECT tt.Id FROM TransferTypes tt WHERE tt.TransferType = 'Send'), (SELECT a.Id FROM Accounts a INNER JOIN Users u ON (a.UserId = u.Id) WHERE u.Username = 'oscar'), (SELECT a.Id FROM Accounts a INNER JOIN Users u ON (a.UserId = u.Id) WHERE u.Username = 'george'), 20, GETDATE());
--INSERT INTO Transfers (TransferStatusId, TransferTypeId, AccountIdFrom, AccountIdTo, Amount, DateCreated) VALUES ((SELECT ts.Id FROM TransferStatuses ts WHERE ts.TransferStatus = 'Pending'), (SELECT tt.Id FROM TransferTypes tt WHERE tt.TransferType = 'Request'), (SELECT a.Id FROM Accounts a INNER JOIN Users u ON (a.UserId = u.Id) WHERE u.Username = 'george'), (SELECT a.Id FROM Accounts a INNER JOIN Users u ON (a.UserId = u.Id) WHERE u.Username = 'brian'), 35, GETDATE());
