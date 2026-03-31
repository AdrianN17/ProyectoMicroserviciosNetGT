-- =========================
-- WALLET
-- =========================
CREATE TABLE Wallet (
                        WalletId INT IDENTITY(1,1) PRIMARY KEY,
                        DocumentNumber VARCHAR(255),
                        DocumentType INT,
                        Currency INT,
                        Status INT,
                        CreatedAt DATETIME,
                        UpdatedAt DATETIME,
                        WalletStatus INT,
                        TypeWalletLimit INT
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
                                   Status INT,
                                   CreatedAt DATETIME,
                                   UpdatedAt DATETIME,

                                   CONSTRAINT FK_WalletTransaction_Wallet
                                       FOREIGN KEY (WalletId) REFERENCES Wallet(WalletId)
);

-- =========================
-- WALLET LIMIT
-- =========================
CREATE TABLE WalletLimit (
                             WalletLimitId INT IDENTITY(1,1) PRIMARY KEY,
                             DailyLimit DECIMAL(18,2)
);

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