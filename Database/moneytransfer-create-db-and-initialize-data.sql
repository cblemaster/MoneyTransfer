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

--INSERT INTO Transfers (TransferStatusId, TransferTypeId, AccountIdFrom, AccountIdTo, Amount, DateCreated) VALUES ((SELECT ts.Id FROM TransferStatuses ts WHERE ts.TransferStatus = 'Approved'), (SELECT tt.Id FROM TransferTypes tt WHERE tt.TransferType = 'Send'), (SELECT a.Id FROM Accounts a INNER JOIN Users u ON (a.UserId = u.Id) WHERE u.Username = 'brian'), (SELECT a.Id FROM Accounts a INNER JOIN Users u ON (a.UserId = u.Id) WHERE u.Username = 'oscar'), 20, GETDATE());
--INSERT INTO Transfers (TransferStatusId, TransferTypeId, AccountIdFrom, AccountIdTo, Amount, DateCreated) VALUES ((SELECT ts.Id FROM TransferStatuses ts WHERE ts.TransferStatus = 'Approved'), (SELECT tt.Id FROM TransferTypes tt WHERE tt.TransferType = 'Request'), (SELECT a.Id FROM Accounts a INNER JOIN Users u ON (a.UserId = u.Id) WHERE u.Username = 'bernice'), (SELECT a.Id FROM Accounts a INNER JOIN Users u ON (a.UserId = u.Id) WHERE u.Username = 'george'), 11, GETDATE());
--INSERT INTO Transfers (TransferStatusId, TransferTypeId, AccountIdFrom, AccountIdTo, Amount, DateCreated) VALUES ((SELECT ts.Id FROM TransferStatuses ts WHERE ts.TransferStatus = 'Approved'), (SELECT tt.Id FROM TransferTypes tt WHERE tt.TransferType = 'Send'), (SELECT a.Id FROM Accounts a INNER JOIN Users u ON (a.UserId = u.Id) WHERE u.Username = 'oscar'), (SELECT a.Id FROM Accounts a INNER JOIN Users u ON (a.UserId = u.Id) WHERE u.Username = 'george'), 7, GETDATE());
--INSERT INTO Transfers (TransferStatusId, TransferTypeId, AccountIdFrom, AccountIdTo, Amount, DateCreated) VALUES ((SELECT ts.Id FROM TransferStatuses ts WHERE ts.TransferStatus = 'Approved'), (SELECT tt.Id FROM TransferTypes tt WHERE tt.TransferType = 'Send'), (SELECT a.Id FROM Accounts a INNER JOIN Users u ON (a.UserId = u.Id) WHERE u.Username = 'george'), (SELECT a.Id FROM Accounts a INNER JOIN Users u ON (a.UserId = u.Id) WHERE u.Username = 'brian'), 12, GETDATE());
--INSERT INTO Transfers (TransferStatusId, TransferTypeId, AccountIdFrom, AccountIdTo, Amount, DateCreated) VALUES ((SELECT ts.Id FROM TransferStatuses ts WHERE ts.TransferStatus = 'Approved'), (SELECT tt.Id FROM TransferTypes tt WHERE tt.TransferType = 'Send'), (SELECT a.Id FROM Accounts a INNER JOIN Users u ON (a.UserId = u.Id) WHERE u.Username = 'brian'), (SELECT a.Id FROM Accounts a INNER JOIN Users u ON (a.UserId = u.Id) WHERE u.Username = 'george'), 16, GETDATE());
--INSERT INTO Transfers (TransferStatusId, TransferTypeId, AccountIdFrom, AccountIdTo, Amount, DateCreated) VALUES ((SELECT ts.Id FROM TransferStatuses ts WHERE ts.TransferStatus = 'Approved'), (SELECT tt.Id FROM TransferTypes tt WHERE tt.TransferType = 'Request'), (SELECT a.Id FROM Accounts a INNER JOIN Users u ON (a.UserId = u.Id) WHERE u.Username = 'bernice'), (SELECT a.Id FROM Accounts a INNER JOIN Users u ON (a.UserId = u.Id) WHERE u.Username = 'oscar'), 14, GETDATE());
--INSERT INTO Transfers (TransferStatusId, TransferTypeId, AccountIdFrom, AccountIdTo, Amount, DateCreated) VALUES ((SELECT ts.Id FROM TransferStatuses ts WHERE ts.TransferStatus = 'Approved'), (SELECT tt.Id FROM TransferTypes tt WHERE tt.TransferType = 'Send'), (SELECT a.Id FROM Accounts a INNER JOIN Users u ON (a.UserId = u.Id) WHERE u.Username = 'oscar'), (SELECT a.Id FROM Accounts a INNER JOIN Users u ON (a.UserId = u.Id) WHERE u.Username = 'bernice'), 2, GETDATE());
--INSERT INTO Transfers (TransferStatusId, TransferTypeId, AccountIdFrom, AccountIdTo, Amount, DateCreated) VALUES ((SELECT ts.Id FROM TransferStatuses ts WHERE ts.TransferStatus = 'Rejected'), (SELECT tt.Id FROM TransferTypes tt WHERE tt.TransferType = 'Request'), (SELECT a.Id FROM Accounts a INNER JOIN Users u ON (a.UserId = u.Id) WHERE u.Username = 'george'), (SELECT a.Id FROM Accounts a INNER JOIN Users u ON (a.UserId = u.Id) WHERE u.Username = 'oscar'), 1, GETDATE());
--INSERT INTO Transfers (TransferStatusId, TransferTypeId, AccountIdFrom, AccountIdTo, Amount, DateCreated) VALUES ((SELECT ts.Id FROM TransferStatuses ts WHERE ts.TransferStatus = 'Approved'), (SELECT tt.Id FROM TransferTypes tt WHERE tt.TransferType = 'Send'), (SELECT a.Id FROM Accounts a INNER JOIN Users u ON (a.UserId = u.Id) WHERE u.Username = 'brian'), (SELECT a.Id FROM Accounts a INNER JOIN Users u ON (a.UserId = u.Id) WHERE u.Username = 'bernice'), 23, GETDATE());
--INSERT INTO Transfers (TransferStatusId, TransferTypeId, AccountIdFrom, AccountIdTo, Amount, DateCreated) VALUES ((SELECT ts.Id FROM TransferStatuses ts WHERE ts.TransferStatus = 'Pending'), (SELECT tt.Id FROM TransferTypes tt WHERE tt.TransferType = 'Request'), (SELECT a.Id FROM Accounts a INNER JOIN Users u ON (a.UserId = u.Id) WHERE u.Username = 'bernice'), (SELECT a.Id FROM Accounts a INNER JOIN Users u ON (a.UserId = u.Id) WHERE u.Username = 'brian'), 15, GETDATE());
--INSERT INTO Transfers (TransferStatusId, TransferTypeId, AccountIdFrom, AccountIdTo, Amount, DateCreated) VALUES ((SELECT ts.Id FROM TransferStatuses ts WHERE ts.TransferStatus = 'Approved'), (SELECT tt.Id FROM TransferTypes tt WHERE tt.TransferType = 'Send'), (SELECT a.Id FROM Accounts a INNER JOIN Users u ON (a.UserId = u.Id) WHERE u.Username = 'oscar'), (SELECT a.Id FROM Accounts a INNER JOIN Users u ON (a.UserId = u.Id) WHERE u.Username = 'bernice'), 5, GETDATE());
--INSERT INTO Transfers (TransferStatusId, TransferTypeId, AccountIdFrom, AccountIdTo, Amount, DateCreated) VALUES ((SELECT ts.Id FROM TransferStatuses ts WHERE ts.TransferStatus = 'Approved'), (SELECT tt.Id FROM TransferTypes tt WHERE tt.TransferType = 'Request'), (SELECT a.Id FROM Accounts a INNER JOIN Users u ON (a.UserId = u.Id) WHERE u.Username = 'george'), (SELECT a.Id FROM Accounts a INNER JOIN Users u ON (a.UserId = u.Id) WHERE u.Username = 'oscar'), 8, GETDATE());
--INSERT INTO Transfers (TransferStatusId, TransferTypeId, AccountIdFrom, AccountIdTo, Amount, DateCreated) VALUES ((SELECT ts.Id FROM TransferStatuses ts WHERE ts.TransferStatus = 'Rejected'), (SELECT tt.Id FROM TransferTypes tt WHERE tt.TransferType = 'Request'), (SELECT a.Id FROM Accounts a INNER JOIN Users u ON (a.UserId = u.Id) WHERE u.Username = 'brian'), (SELECT a.Id FROM Accounts a INNER JOIN Users u ON (a.UserId = u.Id) WHERE u.Username = 'bernice'), 12, GETDATE());
--INSERT INTO Transfers (TransferStatusId, TransferTypeId, AccountIdFrom, AccountIdTo, Amount, DateCreated) VALUES ((SELECT ts.Id FROM TransferStatuses ts WHERE ts.TransferStatus = 'Pending'), (SELECT tt.Id FROM TransferTypes tt WHERE tt.TransferType = 'Request'), (SELECT a.Id FROM Accounts a INNER JOIN Users u ON (a.UserId = u.Id) WHERE u.Username = 'bernice'), (SELECT a.Id FROM Accounts a INNER JOIN Users u ON (a.UserId = u.Id) WHERE u.Username = 'brian'), 21, GETDATE());
--INSERT INTO Transfers (TransferStatusId, TransferTypeId, AccountIdFrom, AccountIdTo, Amount, DateCreated) VALUES ((SELECT ts.Id FROM TransferStatuses ts WHERE ts.TransferStatus = 'Approved'), (SELECT tt.Id FROM TransferTypes tt WHERE tt.TransferType = 'Send'), (SELECT a.Id FROM Accounts a INNER JOIN Users u ON (a.UserId = u.Id) WHERE u.Username = 'oscar'), (SELECT a.Id FROM Accounts a INNER JOIN Users u ON (a.UserId = u.Id) WHERE u.Username = 'brian'), 23, GETDATE());
--INSERT INTO Transfers (TransferStatusId, TransferTypeId, AccountIdFrom, AccountIdTo, Amount, DateCreated) VALUES ((SELECT ts.Id FROM TransferStatuses ts WHERE ts.TransferStatus = 'Rejected'), (SELECT tt.Id FROM TransferTypes tt WHERE tt.TransferType = 'Request'), (SELECT a.Id FROM Accounts a INNER JOIN Users u ON (a.UserId = u.Id) WHERE u.Username = 'george'), (SELECT a.Id FROM Accounts a INNER JOIN Users u ON (a.UserId = u.Id) WHERE u.Username = 'bernice'), 3, GETDATE());
--INSERT INTO Transfers (TransferStatusId, TransferTypeId, AccountIdFrom, AccountIdTo, Amount, DateCreated) VALUES ((SELECT ts.Id FROM TransferStatuses ts WHERE ts.TransferStatus = 'Approved'), (SELECT tt.Id FROM TransferTypes tt WHERE tt.TransferType = 'Send'), (SELECT a.Id FROM Accounts a INNER JOIN Users u ON (a.UserId = u.Id) WHERE u.Username = 'brian'), (SELECT a.Id FROM Accounts a INNER JOIN Users u ON (a.UserId = u.Id) WHERE u.Username = 'oscar'), 7, GETDATE());
--INSERT INTO Transfers (TransferStatusId, TransferTypeId, AccountIdFrom, AccountIdTo, Amount, DateCreated) VALUES ((SELECT ts.Id FROM TransferStatuses ts WHERE ts.TransferStatus = 'Approved'), (SELECT tt.Id FROM TransferTypes tt WHERE tt.TransferType = 'Request'), (SELECT a.Id FROM Accounts a INNER JOIN Users u ON (a.UserId = u.Id) WHERE u.Username = 'george'), (SELECT a.Id FROM Accounts a INNER JOIN Users u ON (a.UserId = u.Id) WHERE u.Username = 'bernice'), 18, GETDATE());
--INSERT INTO Transfers (TransferStatusId, TransferTypeId, AccountIdFrom, AccountIdTo, Amount, DateCreated) VALUES ((SELECT ts.Id FROM TransferStatuses ts WHERE ts.TransferStatus = 'Approved'), (SELECT tt.Id FROM TransferTypes tt WHERE tt.TransferType = 'Send'), (SELECT a.Id FROM Accounts a INNER JOIN Users u ON (a.UserId = u.Id) WHERE u.Username = 'oscar'), (SELECT a.Id FROM Accounts a INNER JOIN Users u ON (a.UserId = u.Id) WHERE u.Username = 'george'), 20, GETDATE());
--INSERT INTO Transfers (TransferStatusId, TransferTypeId, AccountIdFrom, AccountIdTo, Amount, DateCreated) VALUES ((SELECT ts.Id FROM TransferStatuses ts WHERE ts.TransferStatus = 'Pending'), (SELECT tt.Id FROM TransferTypes tt WHERE tt.TransferType = 'Request'), (SELECT a.Id FROM Accounts a INNER JOIN Users u ON (a.UserId = u.Id) WHERE u.Username = 'george'), (SELECT a.Id FROM Accounts a INNER JOIN Users u ON (a.UserId = u.Id) WHERE u.Username = 'brian'), 35, GETDATE());
