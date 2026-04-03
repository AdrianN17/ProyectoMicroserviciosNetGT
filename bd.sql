-- =========================
-- WALLET
-- =========================
CREATE TABLE Wallet (
                        WalletId GUID PRIMARY KEY,
                        Name varchar(50),
                        LastName varchar(50),
                        Email varchar(100),
                        Phone varchar(9),
                        DocumentNumber VARCHAR(20),
                        DocumentType INT,
                        Status INT,
                        CreatedAt DATETIME,
                        UpdatedAt DATETIME,
                        WalletStatus INT,
                        TypeWalletLimit INT
);

CREATE TABLE WalletLimit (
                             WalletLimitId GUID PRIMARY KEY,
                             WalletId GUID,   
                             Currency INT,
                             DailyLimit DECIMAL(18,2)
);

-- =========================
-- WALLET TRANSACTION
-- =========================
CREATE TABLE WalletTransaction (
                                   WalletTransactionId INT IDENTITY(1,1) PRIMARY KEY,
                                   WalletId INT,
                                   Amount DECIMAL(18,2),
                                   PreviousBalance DECIMAL(18,2),
                                   NewBalance DECIMAL(18,2),
                                   PaymentId INT,
                                   Currency INT,
                                   CreatedAt DATETIME,
                                   UpdatedAt DATETIME,

                                   CONSTRAINT FK_WalletTransaction_Wallet
                                       FOREIGN KEY (WalletId) REFERENCES Wallet(WalletId)
);

-- =========================
-- WALLET LIMIT
-- =========================


-- =========================
-- PAYMENT
-- =========================
CREATE TABLE Payment (
                         PaymentId INT IDENTITY(1,1) PRIMARY KEY,
                         paymentStatus INT,
                         FromWalletId INT,
                         ToWalletId INT,
                         Amount DECIMAL(18,2),
                         Currency INT,
                         Status INT,
                         CreatedAt DATETIME,
                         UpdatedAt DATETIME,

                         CONSTRAINT FK_Payment_FromWallet
                             FOREIGN KEY (FromWalletId) REFERENCES Wallet(WalletId),

                         CONSTRAINT FK_Payment_ToWallet
                             FOREIGN KEY (ToWalletId) REFERENCES Wallet(WalletId)
);

-- =========================
-- FK adicional (WalletTransaction -> Payment)
-- =========================
ALTER TABLE WalletTransaction
    ADD CONSTRAINT FK_WalletTransaction_Payment
        FOREIGN KEY (PaymentId) REFERENCES Payment(PaymentId);