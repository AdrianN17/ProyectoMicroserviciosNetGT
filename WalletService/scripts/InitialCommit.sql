IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260405160746_InitialCommit'
)
BEGIN
    IF SCHEMA_ID(N'Wallet') IS NULL EXEC(N'CREATE SCHEMA [Wallet];');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260405160746_InitialCommit'
)
BEGIN
    CREATE TABLE [Wallet].[Wallet] (
        [WalletId] uniqueidentifier NOT NULL,
        [Name] varchar(50) NOT NULL,
        [LastName] varchar(50) NOT NULL,
        [Email] varchar(100) NOT NULL,
        [Phone] varchar(15) NOT NULL,
        [DocumentNumber] varchar(20) NOT NULL,
        [DocumentType] int NOT NULL,
        [WalletStatus] int NOT NULL,
        [CreatedAt] datetime NOT NULL,
        [LastModifiedAt] datetime NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetime NULL,
        CONSTRAINT [PK_Wallet] PRIMARY KEY ([WalletId])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260405160746_InitialCommit'
)
BEGIN
    CREATE TABLE [Wallet].[WalletBalance] (
        [WalletBalanceId] uniqueidentifier NOT NULL,
        [WalletId] uniqueidentifier NOT NULL,
        [Currency] int NOT NULL,
        [BalanceAmount] decimal(18,2) NOT NULL,
        [CreatedAt] datetime NOT NULL,
        [LastModifiedAt] datetime NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetime NULL,
        CONSTRAINT [PK_WalletBalance] PRIMARY KEY ([WalletBalanceId]),
        CONSTRAINT [FK_WalletBalance_Wallet] FOREIGN KEY ([WalletId]) REFERENCES [Wallet].[Wallet] ([WalletId]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260405160746_InitialCommit'
)
BEGIN
    CREATE TABLE [Wallet].[WalletLimit] (
        [WalletLimitId] uniqueidentifier NOT NULL,
        [WalletId] uniqueidentifier NOT NULL,
        [Currency] int NOT NULL,
        [DailyLimit] decimal(18,2) NOT NULL,
        [CreatedAt] datetime NOT NULL,
        [LastModifiedAt] datetime NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetime NULL,
        CONSTRAINT [PK_WalletLimit] PRIMARY KEY ([WalletLimitId]),
        CONSTRAINT [FK_WalletLimit_Wallet] FOREIGN KEY ([WalletId]) REFERENCES [Wallet].[Wallet] ([WalletId]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260405160746_InitialCommit'
)
BEGIN
    CREATE UNIQUE INDEX [IX_WalletBalance_WalletId] ON [Wallet].[WalletBalance] ([WalletId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260405160746_InitialCommit'
)
BEGIN
    CREATE UNIQUE INDEX [IX_WalletLimit_WalletId] ON [Wallet].[WalletLimit] ([WalletId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260405160746_InitialCommit'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260405160746_InitialCommit', N'10.0.3');
END;

COMMIT;
GO

