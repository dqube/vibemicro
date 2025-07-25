USE [Storesdb]
/******************** SCHEMA CREATION ********************/
GO
CREATE SCHEMA shared;
GO
CREATE SCHEMA messaging;
GO
CREATE SCHEMA auth;
GO
CREATE SCHEMA contact;
GO
CREATE SCHEMA store;
GO
CREATE SCHEMA employee;
GO
CREATE SCHEMA catalog;
GO
CREATE SCHEMA inventory;
GO
CREATE SCHEMA supplier;
GO
CREATE SCHEMA promotion;
GO
CREATE SCHEMA customer;
GO
CREATE SCHEMA [sales];
GO
CREATE SCHEMA payment;
GO
CREATE SCHEMA reporting;
GO

/******************** SHARED FOUNDATIONAL TABLES ********************/
-- Must be created first as they're referenced by most other tables
CREATE TABLE shared.Currencies (
    CurrencyCode CHAR(3) PRIMARY KEY,
    Name VARCHAR(50) NOT NULL,
    Symbol VARCHAR(5) NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    CreatedBy UNIQUEIDENTIFIER NULL,
    UpdatedAt DATETIME2 NULL,
    UpdatedBy UNIQUEIDENTIFIER NULL
);

CREATE TABLE shared.Countries (
    CountryCode CHAR(2) PRIMARY KEY,
    Name VARCHAR(100) NOT NULL,
    CurrencyCode CHAR(3) NOT NULL FOREIGN KEY REFERENCES shared.Currencies(CurrencyCode),
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    CreatedBy UNIQUEIDENTIFIER NULL,
    UpdatedAt DATETIME2 NULL,
    UpdatedBy UNIQUEIDENTIFIER NULL
);

CREATE TABLE contact.ContactNumberTypes (
    ContactNumberTypeId INT PRIMARY KEY,
    Name VARCHAR(50) NOT NULL UNIQUE,
    Description VARCHAR(255) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    CreatedBy UNIQUEIDENTIFIER NULL,
    UpdatedAt DATETIME2 NULL,
    UpdatedBy UNIQUEIDENTIFIER NULL
);

CREATE TABLE contact.AddressTypes (
    AddressTypeId INT PRIMARY KEY,
    Name VARCHAR(50) NOT NULL UNIQUE,
    Description VARCHAR(255) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    CreatedBy UNIQUEIDENTIFIER NULL,
    UpdatedAt DATETIME2 NULL,
    UpdatedBy UNIQUEIDENTIFIER NULL
);

/******************** auth & ACCESS TABLES ********************/
CREATE TABLE auth.Roles (
    RoleId INT PRIMARY KEY,
    Name VARCHAR(20) NOT NULL UNIQUE,
    Description VARCHAR(255) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    CreatedBy UNIQUEIDENTIFIER NULL,
    UpdatedAt DATETIME2 NULL,
    UpdatedBy UNIQUEIDENTIFIER NULL
);

CREATE TABLE auth.Users (
    UserId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Username VARCHAR(50) NOT NULL UNIQUE,
    Email VARCHAR(100) NOT NULL UNIQUE,
    PasswordHash VARBINARY(MAX) NOT NULL,
    PasswordSalt VARBINARY(MAX) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    CreatedBy UNIQUEIDENTIFIER NULL,
    UpdatedAt DATETIME2 NULL,
    UpdatedBy UNIQUEIDENTIFIER NULL,
    FailedLoginAttempts INT NOT NULL DEFAULT 0,
    LockoutEnd DATETIME2 NULL
);

CREATE TABLE auth.UserRoles (
    UserId UNIQUEIDENTIFIER FOREIGN KEY REFERENCES auth.Users(UserId),
    RoleId INT FOREIGN KEY REFERENCES auth.Roles(RoleId),
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    CreatedBy UNIQUEIDENTIFIER NULL,
    PRIMARY KEY (UserId, RoleId)
);

CREATE TABLE auth.RegistrationTokens (
    TokenId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES auth.Users(UserId),
    TokenType VARCHAR(20) NOT NULL CHECK (TokenType IN ('EmailVerification','PasswordReset')),
    Expiration DATETIME2 NOT NULL DEFAULT DATEADD(HOUR, 24, GETDATE()),
    IsUsed BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    CreatedBy UNIQUEIDENTIFIER NULL
);

/******************** STORE STRUCTURE TABLES ********************/
CREATE TABLE store.Stores (
    StoreId INT PRIMARY KEY IDENTITY(1,1),
    Name VARCHAR(100) NOT NULL,
    LocationId INT NOT NULL,
    Address VARCHAR(255) NOT NULL,
    Phone VARCHAR(20) NOT NULL,
    OpeningHours VARCHAR(100) NOT NULL,
    Status VARCHAR(20) NOT NULL CHECK (Status IN ('Active','Maintenance','Closed')) DEFAULT 'Active',
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    CreatedBy UNIQUEIDENTIFIER NULL,
    UpdatedAt DATETIME2 NULL,
    UpdatedBy UNIQUEIDENTIFIER NULL
);

CREATE TABLE store.Registers (
    RegisterId INT PRIMARY KEY IDENTITY(1,1),
    StoreId INT NOT NULL FOREIGN KEY REFERENCES store.Stores(StoreId),
    Name VARCHAR(50) NOT NULL,
    CurrentBalance DECIMAL(19,4) NOT NULL DEFAULT 0,
    Status VARCHAR(20) NOT NULL CHECK (Status IN ('Open','Closed')) DEFAULT 'Closed',
    LastOpen DATETIME2 NULL,
    LastClose DATETIME2 NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    CreatedBy UNIQUEIDENTIFIER NULL,
    UpdatedAt DATETIME2 NULL,
    UpdatedBy UNIQUEIDENTIFIER NULL
);

CREATE TABLE store.Shifts (
    ShiftId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    EmployeeId UNIQUEIDENTIFIER NOT NULL,
    RegisterId INT NOT NULL FOREIGN KEY REFERENCES store.Registers(RegisterId),
    StartTime DATETIME2 NOT NULL DEFAULT GETDATE(),
    EndTime DATETIME2 NULL,
    StartingCash DECIMAL(19,4) NOT NULL,
    EndingCash DECIMAL(19,4) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    CreatedBy UNIQUEIDENTIFIER NULL,
    UpdatedAt DATETIME2 NULL,
    UpdatedBy UNIQUEIDENTIFIER NULL
);

CREATE TABLE store.CashDrawerMovements (
    MovementId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    RegisterId INT NOT NULL FOREIGN KEY REFERENCES store.Registers(RegisterId),
    EmployeeId UNIQUEIDENTIFIER NOT NULL,
    MovementType VARCHAR(20) NOT NULL CHECK (MovementType IN ('Open','Close','CashIn','CashOut')),
    Amount DECIMAL(19,4) NOT NULL,
    MovementTime DATETIME2 NOT NULL DEFAULT GETDATE(),
    Note VARCHAR(255) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    CreatedBy UNIQUEIDENTIFIER NULL
);

/******************** EMPLOYEE MANAGEMENT TABLES ********************/
CREATE TABLE employee.Employees (
    EmployeeId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserId UNIQUEIDENTIFIER NOT NULL UNIQUE FOREIGN KEY REFERENCES auth.Users(UserId),
    StoreId INT NOT NULL FOREIGN KEY REFERENCES store.Stores(StoreId),
    EmployeeNumber VARCHAR(20) NOT NULL UNIQUE,
    HireDate DATE NOT NULL DEFAULT CAST(GETDATE() AS DATE),
    TerminationDate DATE NULL,
    Position VARCHAR(50) NOT NULL,
    AuthLevel INT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    CreatedBy UNIQUEIDENTIFIER NULL,
    UpdatedAt DATETIME2 NULL,
    UpdatedBy UNIQUEIDENTIFIER NULL
);

CREATE TABLE employee.EmployeeContactNumbers (
    ContactNumberId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    EmployeeId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES employee.Employees(EmployeeId),
    ContactNumberTypeId INT NOT NULL FOREIGN KEY REFERENCES contact.ContactNumberTypes(ContactNumberTypeId),
    PhoneNumber VARCHAR(20) NOT NULL,
    IsPrimary BIT NOT NULL DEFAULT 0,
    Verified BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME2 NULL,
    UpdatedBy UNIQUEIDENTIFIER NULL
);

CREATE TABLE employee.EmployeeAddresses (
    AddressId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    EmployeeId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES employee.Employees(EmployeeId),
    AddressTypeId INT NOT NULL FOREIGN KEY REFERENCES contact.AddressTypes(AddressTypeId),
    Line1 VARCHAR(100) NOT NULL,
    Line2 VARCHAR(100) NULL,
    City VARCHAR(50) NOT NULL,
    State VARCHAR(50) NULL,
    PostalCode VARCHAR(20) NOT NULL,
    CountryCode CHAR(2) NOT NULL FOREIGN KEY REFERENCES shared.Countries(CountryCode),
    IsPrimary BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME2 NULL,
    UpdatedBy UNIQUEIDENTIFIER NULL
);

/******************** PRODUCT CATALOG TABLES ********************/
CREATE TABLE catalog.ProductCategories (
    CategoryId INT PRIMARY KEY IDENTITY(1,1),
    Name VARCHAR(100) NOT NULL UNIQUE,
    ParentCategoryId INT NULL FOREIGN KEY REFERENCES catalog.ProductCategories(CategoryId),
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    CreatedBy UNIQUEIDENTIFIER NULL,
    UpdatedAt DATETIME2 NULL,
    UpdatedBy UNIQUEIDENTIFIER NULL
);

CREATE TABLE catalog.Products (
    ProductId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    SKU VARCHAR(50) NOT NULL UNIQUE,
    Name VARCHAR(255) NOT NULL,
    Description TEXT NULL,
    CategoryId INT NOT NULL FOREIGN KEY REFERENCES catalog.ProductCategories(CategoryId),
    BasePrice DECIMAL(19,4) NOT NULL CHECK (BasePrice >= 0),
    CostPrice DECIMAL(19,4) NOT NULL CHECK (CostPrice >= 0),
    IsTaxable BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    CreatedBy UNIQUEIDENTIFIER NULL,
    UpdatedAt DATETIME2 NULL,
    UpdatedBy UNIQUEIDENTIFIER NULL
);

CREATE TABLE catalog.ProductBarcodes (
    BarcodeId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    ProductId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES catalog.Products(ProductId),
    BarcodeValue VARCHAR(50) NOT NULL UNIQUE,
    BarcodeType VARCHAR(20) NOT NULL DEFAULT 'UPC-A',
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    CreatedBy UNIQUEIDENTIFIER NULL
);

CREATE TABLE catalog.CountryPricing (
    PricingId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    ProductId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES catalog.Products(ProductId),
    CountryCode CHAR(2) NOT NULL FOREIGN KEY REFERENCES shared.Countries(CountryCode),
    Price DECIMAL(19,4) NOT NULL CHECK (Price >= 0),
    EffectiveDate DATE NOT NULL DEFAULT CAST(GETDATE() AS DATE),
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    CreatedBy UNIQUEIDENTIFIER NULL,
    UNIQUE (ProductId, CountryCode)
);

CREATE TABLE catalog.TaxConfigurations (
    TaxConfigId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    LocationId INT NOT NULL,
    CategoryId INT NULL FOREIGN KEY REFERENCES catalog.ProductCategories(CategoryId),
    TaxRate DECIMAL(5,2) NOT NULL CHECK (TaxRate >= 0),
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    CreatedBy UNIQUEIDENTIFIER NULL,
    UpdatedAt DATETIME2 NULL,
    UpdatedBy UNIQUEIDENTIFIER NULL
);

/******************** INVENTORY MANAGEMENT TABLES ********************/
CREATE TABLE inventory.InventoryItems (
    InventoryItemId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    StoreId INT NOT NULL,
    ProductId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES catalog.Products(ProductId),
    Quantity INT NOT NULL CHECK (Quantity >= 0) DEFAULT 0,
    ReorderLevel INT NOT NULL DEFAULT 10,
    LastRestockDate DATETIME2 NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    CreatedBy UNIQUEIDENTIFIER NULL,
    UpdatedAt DATETIME2 NULL,
    UpdatedBy UNIQUEIDENTIFIER NULL,
    UNIQUE (StoreId, ProductId)
);

CREATE TABLE inventory.StockMovements (
    MovementId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    ProductId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES catalog.Products(ProductId),
    StoreId INT NOT NULL,
    QuantityChange INT NOT NULL,
    MovementType VARCHAR(20) NOT NULL CHECK (MovementType IN ('Purchase','Return','Adjustment','Damage','Transfer')),
    MovementDate DATETIME2 NOT NULL DEFAULT GETDATE(),
    EmployeeId UNIQUEIDENTIFIER NOT NULL,
    ReferenceId UNIQUEIDENTIFIER NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    CreatedBy UNIQUEIDENTIFIER NULL
);

/******************** SUPPLIER MANAGEMENT TABLES ********************/
CREATE TABLE supplier.Suppliers (
    SupplierId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Name VARCHAR(100) NOT NULL,
    TaxIdentificationNumber VARCHAR(50) NULL,
    Website VARCHAR(255) NULL,
    Notes TEXT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    CreatedBy UNIQUEIDENTIFIER NULL,
    UpdatedAt DATETIME2 NULL,
    UpdatedBy UNIQUEIDENTIFIER NULL
);

CREATE TABLE supplier.SupplierContacts (
    ContactId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    SupplierId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES supplier.Suppliers(SupplierId),
    FirstName VARCHAR(50) NOT NULL,
    LastName VARCHAR(50) NOT NULL,
    Email VARCHAR(100) NULL,
    Position VARCHAR(100) NULL,
    IsPrimary BIT NOT NULL DEFAULT 0,
    Notes TEXT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    CreatedBy UNIQUEIDENTIFIER NULL,
    UpdatedAt DATETIME2 NULL,
    UpdatedBy UNIQUEIDENTIFIER NULL
);

CREATE TABLE supplier.SupplierContactNumbers (
    ContactNumberId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    ContactId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES supplier.SupplierContacts(ContactId),
    ContactNumberTypeId INT NOT NULL FOREIGN KEY REFERENCES contact.ContactNumberTypes(ContactNumberTypeId),
    PhoneNumber VARCHAR(20) NOT NULL,
    IsPrimary BIT NOT NULL DEFAULT 0,
    Notes VARCHAR(255) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    CreatedBy UNIQUEIDENTIFIER NULL,
    UpdatedAt DATETIME2 NULL,
    UpdatedBy UNIQUEIDENTIFIER NULL
);

CREATE TABLE supplier.SupplierAddresses (
    AddressId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    SupplierId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES supplier.Suppliers(SupplierId),
    AddressTypeId INT NOT NULL FOREIGN KEY REFERENCES contact.AddressTypes(AddressTypeId),
    Line1 VARCHAR(100) NOT NULL,
    Line2 VARCHAR(100) NULL,
    City VARCHAR(50) NOT NULL,
    State VARCHAR(50) NULL,
    PostalCode VARCHAR(20) NOT NULL,
    CountryCode CHAR(2) NOT NULL FOREIGN KEY REFERENCES shared.Countries(CountryCode),
    IsPrimary BIT NOT NULL DEFAULT 0,
    IsShipping BIT NOT NULL DEFAULT 0,
    IsBilling BIT NOT NULL DEFAULT 0,
    Notes TEXT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    CreatedBy UNIQUEIDENTIFIER NULL,
    UpdatedAt DATETIME2 NULL,
    UpdatedBy UNIQUEIDENTIFIER NULL
);

CREATE TABLE supplier.PurchaseOrders (
    OrderId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    SupplierId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES supplier.Suppliers(SupplierId),
    StoreId INT NOT NULL,
    OrderDate DATETIME2 NOT NULL DEFAULT GETDATE(),
    ExpectedDate DATE NULL,
    Status VARCHAR(20) NOT NULL CHECK (Status IN ('Draft','Ordered','Received','Cancelled')) DEFAULT 'Draft',
    TotalAmount DECIMAL(19,4) NOT NULL DEFAULT 0,
    ShippingAddressId UNIQUEIDENTIFIER NULL FOREIGN KEY REFERENCES supplier.SupplierAddresses(AddressId),
    ContactPersonId UNIQUEIDENTIFIER NULL FOREIGN KEY REFERENCES supplier.SupplierContacts(ContactId),
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    CreatedBy UNIQUEIDENTIFIER NULL,
    UpdatedAt DATETIME2 NULL,
    UpdatedBy UNIQUEIDENTIFIER NULL
);

CREATE TABLE supplier.PurchaseOrderDetails (
    OrderDetailId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    OrderId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES supplier.PurchaseOrders(OrderId),
    ProductId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES catalog.Products(ProductId),
    Quantity INT NOT NULL CHECK (Quantity > 0),
    UnitCost DECIMAL(19,4) NOT NULL CHECK (UnitCost >= 0),
    ReceivedQuantity INT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    CreatedBy UNIQUEIDENTIFIER NULL
);

/******************** PROMOTIONS ENGINE TABLES ********************/
CREATE TABLE promotion.DiscountTypes (
    DiscountTypeId INT PRIMARY KEY,
    Name VARCHAR(50) NOT NULL UNIQUE,
    Description VARCHAR(255) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    CreatedBy UNIQUEIDENTIFIER NULL,
    UpdatedAt DATETIME2 NULL,
    UpdatedBy UNIQUEIDENTIFIER NULL
);

CREATE TABLE promotion.DiscountCampaigns (
    CampaignId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Name VARCHAR(100) NOT NULL,
    Description VARCHAR(255) NULL,
    StartDate DATETIME2 NOT NULL,
    EndDate DATETIME2 NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    MaxUsesPerCustomer INT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    CreatedBy UNIQUEIDENTIFIER NULL,
    UpdatedAt DATETIME2 NULL,
    UpdatedBy UNIQUEIDENTIFIER NULL,
    CHECK (EndDate > StartDate)
);

CREATE TABLE promotion.DiscountRules (
    RuleId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CampaignId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES promotion.DiscountCampaigns(CampaignId),
    RuleType VARCHAR(20) NOT NULL CHECK (RuleType IN ('Category','Product','TotalAmount','BuyXGetY')),
    ProductId UNIQUEIDENTIFIER NULL,
    CategoryId INT NULL,
    MinQuantity INT NULL,
    MinAmount DECIMAL(19,4) NULL,
    DiscountValue DECIMAL(19,4) NOT NULL,
    DiscountType VARCHAR(10) NOT NULL CHECK (DiscountType IN ('Percent','Fixed','FreeItem')) DEFAULT 'Percent',
    FreeProductId UNIQUEIDENTIFIER NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    CreatedBy UNIQUEIDENTIFIER NULL,
    UpdatedAt DATETIME2 NULL,
    UpdatedBy UNIQUEIDENTIFIER NULL
);

CREATE TABLE promotion.Promotions (
    PromotionId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Name VARCHAR(100) NOT NULL,
    Description TEXT NULL,
    StartDate DATETIME2 NOT NULL,
    EndDate DATETIME2 NOT NULL,
    IsCombinable BIT NOT NULL DEFAULT 0,
    MaxRedemptions INT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    CreatedBy UNIQUEIDENTIFIER NULL,
    UpdatedAt DATETIME2 NULL,
    UpdatedBy UNIQUEIDENTIFIER NULL,
    CHECK (EndDate > StartDate)
);

CREATE TABLE promotion.PromotionProducts (
    PromotionProductId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PromotionId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES promotion.Promotions(PromotionId),
    ProductId UNIQUEIDENTIFIER NULL,
    CategoryId INT NULL,
    MinQuantity INT NOT NULL DEFAULT 1,
    DiscountPercent DECIMAL(5,2) NULL,
    BundlePrice DECIMAL(19,4) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    CreatedBy UNIQUEIDENTIFIER NULL
);

/******************** CUSTOMER MANAGEMENT TABLES ********************/
CREATE TABLE customer.Customers (
    CustomerId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserId UNIQUEIDENTIFIER NULL FOREIGN KEY REFERENCES auth.Users(UserId),
    FirstName VARCHAR(50) NOT NULL,
    LastName VARCHAR(50) NOT NULL,
    Email VARCHAR(100) UNIQUE,
    MembershipNumber VARCHAR(50) UNIQUE,
    JoinDate DATE NOT NULL DEFAULT CAST(GETDATE() AS DATE),
    ExpiryDate DATE NOT NULL,
    CountryCode CHAR(2) NOT NULL FOREIGN KEY REFERENCES shared.Countries(CountryCode),
    LoyaltyPoints INT NOT NULL DEFAULT 0,
    PreferredContactMethod INT NULL FOREIGN KEY REFERENCES contact.ContactNumberTypes(ContactNumberTypeId),
    PreferredAddressType INT NULL FOREIGN KEY REFERENCES contact.AddressTypes(AddressTypeId),
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    CreatedBy UNIQUEIDENTIFIER NULL,
    UpdatedAt DATETIME2 NULL,
    UpdatedBy UNIQUEIDENTIFIER NULL
);

CREATE TABLE customer.CustomerContactNumbers (
    ContactNumberId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CustomerId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES customer.Customers(CustomerId),
    ContactNumberTypeId INT NOT NULL FOREIGN KEY REFERENCES contact.ContactNumberTypes(ContactNumberTypeId),
    PhoneNumber VARCHAR(20) NOT NULL,
    IsPrimary BIT NOT NULL DEFAULT 0,
    Verified BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME2 NULL,
    UpdatedBy UNIQUEIDENTIFIER NULL
);

CREATE TABLE customer.CustomerAddresses (
    AddressId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CustomerId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES customer.Customers(CustomerId),
    AddressTypeId INT NOT NULL FOREIGN KEY REFERENCES contact.AddressTypes(AddressTypeId),
    Line1 VARCHAR(100) NOT NULL,
    Line2 VARCHAR(100) NULL,
    City VARCHAR(50) NOT NULL,
    State VARCHAR(50) NULL,
    PostalCode VARCHAR(20) NOT NULL,
    CountryCode CHAR(2) NOT NULL FOREIGN KEY REFERENCES shared.Countries(CountryCode),
    IsPrimary BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME2 NULL,
    UpdatedBy UNIQUEIDENTIFIER NULL
);

CREATE TABLE customer.LoyaltyPrograms (
    ProgramId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Name VARCHAR(100) NOT NULL,
    PointsPerDollar DECIMAL(5,2) NOT NULL DEFAULT 1.0,
    SignupBonus INT NOT NULL DEFAULT 0,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    CreatedBy UNIQUEIDENTIFIER NULL,
    UpdatedAt DATETIME2 NULL,
    UpdatedBy UNIQUEIDENTIFIER NULL
);

CREATE TABLE customer.LoyaltyTiers (
    TierId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    ProgramId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES customer.LoyaltyPrograms(ProgramId),
    Name VARCHAR(50) NOT NULL,
    MinPoints INT NOT NULL,
    DiscountPercent DECIMAL(5,2) NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    CreatedBy UNIQUEIDENTIFIER NULL,
    UpdatedAt DATETIME2 NULL,
    UpdatedBy UNIQUEIDENTIFIER NULL
);

CREATE TABLE customer.GiftCards (
    GiftCardId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CardNumber VARCHAR(20) NOT NULL UNIQUE,
    InitialBalance DECIMAL(19,4) NOT NULL,
    CurrentBalance DECIMAL(19,4) NOT NULL,
    IssueDate DATE NOT NULL DEFAULT CAST(GETDATE() AS DATE),
    ExpiryDate DATE NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    CreatedBy UNIQUEIDENTIFIER NULL,
    UpdatedAt DATETIME2 NULL,
    UpdatedBy UNIQUEIDENTIFIER NULL
);

CREATE TABLE customer.LoyaltyPointLedger (
    LedgerId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CustomerId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES customer.Customers(CustomerId),
    TransactionDate DATETIME2 NOT NULL DEFAULT GETDATE(),
    PointsEarned INT NOT NULL DEFAULT 0,
    PointsRedeemed INT NOT NULL DEFAULT 0,
    SaleId UNIQUEIDENTIFIER NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    CreatedBy UNIQUEIDENTIFIER NULL
);

/******************** TRANSACTION PROCESSING TABLES ********************/
CREATE TABLE sales.Sales (
    SaleId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    StoreId INT NOT NULL,
    EmployeeId UNIQUEIDENTIFIER NOT NULL,
    CustomerId UNIQUEIDENTIFIER NULL FOREIGN KEY REFERENCES customer.Customers(CustomerId),
    RegisterId INT NOT NULL,
    TransactionTime DATETIME2 NOT NULL DEFAULT GETDATE(),
    SubTotal DECIMAL(19,4) NOT NULL CHECK (SubTotal >= 0),
    DiscountTotal DECIMAL(19,4) NOT NULL DEFAULT 0 CHECK (DiscountTotal >= 0),
    TaxAmount DECIMAL(19,4) NOT NULL CHECK (TaxAmount >= 0),
    TotalAmount DECIMAL(19,4) NOT NULL CHECK (TotalAmount >= 0),
    ReceiptNumber VARCHAR(20) NOT NULL UNIQUE,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    CreatedBy UNIQUEIDENTIFIER NULL
);

CREATE TABLE sales.SaleDetails (
    SaleDetailId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    SaleId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES sales.Sales(SaleId),
    ProductId UNIQUEIDENTIFIER NOT NULL,
    Quantity INT NOT NULL CHECK (Quantity > 0),
    UnitPrice DECIMAL(19,4) NOT NULL CHECK (UnitPrice >= 0),
    AppliedDiscount DECIMAL(19,4) NOT NULL DEFAULT 0,
    TaxApplied DECIMAL(19,4) NOT NULL CHECK (TaxApplied >= 0),
    LineTotal DECIMAL(19,4) NOT NULL CHECK (LineTotal >= 0),
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    CreatedBy UNIQUEIDENTIFIER NULL
);

CREATE TABLE sales.AppliedDiscounts (
    AppliedDiscountId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    SaleDetailId UNIQUEIDENTIFIER NULL FOREIGN KEY REFERENCES sales.SaleDetails(SaleDetailId),
    SaleId UNIQUEIDENTIFIER NULL FOREIGN KEY REFERENCES sales.Sales(SaleId),
    CampaignId UNIQUEIDENTIFIER NOT NULL,
    RuleId UNIQUEIDENTIFIER NOT NULL,
    DiscountAmount DECIMAL(19,4) NOT NULL CHECK (DiscountAmount >= 0),
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    CreatedBy UNIQUEIDENTIFIER NULL,
    CHECK (
        (SaleDetailId IS NOT NULL AND SaleId IS NULL) OR 
        (SaleDetailId IS NULL AND SaleId IS NOT NULL)
    )
);

CREATE TABLE sales.Returns (
    ReturnId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    SaleId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES sales.Sales(SaleId),
    ReturnDate DATETIME2 NOT NULL DEFAULT GETDATE(),
    EmployeeId UNIQUEIDENTIFIER NOT NULL,
    CustomerId UNIQUEIDENTIFIER NULL FOREIGN KEY REFERENCES customer.Customers(CustomerId),
    TotalRefund DECIMAL(19,4) NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    CreatedBy UNIQUEIDENTIFIER NULL
);

CREATE TABLE sales.ReturnDetails (
    ReturnDetailId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    ReturnId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES sales.Returns(ReturnId),
    ProductId UNIQUEIDENTIFIER NOT NULL,
    Quantity INT NOT NULL,
    Reason VARCHAR(50) NOT NULL CHECK (Reason IN ('Defective','WrongItem','CustomerChange','Other')),
    Restock BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    CreatedBy UNIQUEIDENTIFIER NULL
);

/******************** PAYMENT PROCESSING TABLES ********************/
CREATE TABLE payment.PaymentTypes (
    PaymentTypeId INT PRIMARY KEY,
    Name VARCHAR(50) NOT NULL UNIQUE,
    Description VARCHAR(255) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    CreatedBy UNIQUEIDENTIFIER NULL,
    UpdatedAt DATETIME2 NULL,
    UpdatedBy UNIQUEIDENTIFIER NULL
);

CREATE TABLE payment.PaymentProcessors (
    ProcessorId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Name VARCHAR(50) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CommissionRate DECIMAL(5,2) NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    CreatedBy UNIQUEIDENTIFIER NULL,
    UpdatedAt DATETIME2 NULL,
    UpdatedBy UNIQUEIDENTIFIER NULL
);

CREATE TABLE payment.PaymentMethods (
    MethodId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PaymentTypeId INT NOT NULL FOREIGN KEY REFERENCES payment.PaymentTypes(PaymentTypeId),
    ProcessorId UNIQUEIDENTIFIER NULL FOREIGN KEY REFERENCES payment.PaymentProcessors(ProcessorId),
    Name VARCHAR(50) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    CreatedBy UNIQUEIDENTIFIER NULL,
    UpdatedAt DATETIME2 NULL,
    UpdatedBy UNIQUEIDENTIFIER NULL
);

CREATE TABLE payment.SalePayments (
    PaymentId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    SaleId UNIQUEIDENTIFIER NOT NULL,
    MethodId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES payment.PaymentMethods(MethodId),
    Amount DECIMAL(19,4) NOT NULL CHECK (Amount > 0),
    TransactionCode VARCHAR(100) NULL,
    ApprovalCode VARCHAR(50) NULL,
    ProcessedTime DATETIME2 NOT NULL DEFAULT GETDATE(),
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    CreatedBy UNIQUEIDENTIFIER NULL
);

CREATE TABLE payment.PaymentDetails (
    PaymentDetailId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    SalePaymentId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES payment.SalePayments(PaymentId),
    CardLastFour CHAR(4) NULL,
    CardType VARCHAR(20) NULL,
    AuthorizationCode VARCHAR(50) NULL,
    ProcessorResponse TEXT NULL,
    IsSettled BIT NOT NULL DEFAULT 0,
    SettlementDate DATETIME2 NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    CreatedBy UNIQUEIDENTIFIER NULL,
    UpdatedAt DATETIME2 NULL,
    UpdatedBy UNIQUEIDENTIFIER NULL
);

CREATE TABLE payment.GiftCardTransactions (
    TransactionId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    GiftCardId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES customer.GiftCards(GiftCardId),
    SalePaymentId UNIQUEIDENTIFIER NULL FOREIGN KEY REFERENCES payment.SalePayments(PaymentId),
    Amount DECIMAL(19,4) NOT NULL,
    TransactionType VARCHAR(20) NOT NULL CHECK (TransactionType IN ('Issuance','Redemption','Reload')),
    TransactionDate DATETIME2 NOT NULL DEFAULT GETDATE(),
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    CreatedBy UNIQUEIDENTIFIER NULL
);

/******************** REPORTING & ANALYTICS TABLES ********************/
CREATE TABLE reporting.SalesSnapshots (
    SnapshotId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    SaleId UNIQUEIDENTIFIER NOT NULL,
    StoreId INT NOT NULL,
    SaleDate DATE NOT NULL,
    TotalAmount DECIMAL(19,4) NOT NULL,
    CustomerId UNIQUEIDENTIFIER NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    CreatedBy UNIQUEIDENTIFIER NULL
);

CREATE TABLE reporting.InventorySnapshots (
    SnapshotId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    ProductId UNIQUEIDENTIFIER NOT NULL,
    StoreId INT NOT NULL,
    Quantity INT NOT NULL,
    SnapshotDate DATE NOT NULL DEFAULT CAST(GETDATE() AS DATE),
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    CreatedBy UNIQUEIDENTIFIER NULL
);

CREATE TABLE reporting.PromotionEffectiveness (
    EffectivenessId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PromotionId UNIQUEIDENTIFIER NOT NULL,
    RedemptionCount INT NOT NULL DEFAULT 0,
    RevenueImpact DECIMAL(19,4) NOT NULL,
    AnalysisDate DATE NOT NULL DEFAULT CAST(GETDATE() AS DATE),
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    CreatedBy UNIQUEIDENTIFIER NULL
);

/******************** EVENT MESSAGING TABLES ********************/
CREATE TABLE messaging.Outbox (
    MessageId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    EventType VARCHAR(100) NOT NULL,
    Payload TEXT NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    CreatedBy UNIQUEIDENTIFIER NULL,
    ProcessedDate DATETIME2 NULL
);

CREATE TABLE messaging.Inbox (
    MessageId UNIQUEIDENTIFIER PRIMARY KEY,
    EventType VARCHAR(100) NOT NULL,
    Payload TEXT NOT NULL,
    ReceivedDate DATETIME2 NOT NULL DEFAULT GETDATE(),
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    CreatedBy UNIQUEIDENTIFIER NULL,
    ProcessedDate DATETIME2 NULL
);

/******************** REFERENCE DATA POPULATION ********************/
-- Currencies (must be first as referenced by countries)
INSERT INTO shared.Currencies (CurrencyCode, Name, Symbol) VALUES
('USD', 'US Dollar', '$'),
('EUR', 'Euro', '€'),
('GBP', 'British Pound', '£'),
('JPY', 'Japanese Yen', '¥'),
('CAD', 'Canadian Dollar', '$'),
('AUD', 'Australian Dollar', '$');

-- Countries (referenced by addresses)
INSERT INTO shared.Countries (CountryCode, Name, CurrencyCode) VALUES
('US', 'United States', 'USD'),
('GB', 'United Kingdom', 'GBP'),
('DE', 'Germany', 'EUR'),
('FR', 'France', 'EUR'),
('CA', 'Canada', 'CAD'),
('AU', 'Australia', 'AUD'),
('JP', 'Japan', 'JPY');

-- Contact number types (used by multiple domains)
INSERT INTO contact.ContactNumberTypes (ContactNumberTypeId, Name, Description) VALUES
(1, 'Mobile', 'Primary mobile number'),
(2, 'Home', 'Home landline number'),
(3, 'Work', 'Work contact number'),
(4, 'Emergency', 'Emergency contact number'),
(5, 'Fax', 'Facsimile number'),
(6, 'Other', 'Other contact number');

-- Address types (used by multiple domains)
INSERT INTO contact.AddressTypes (AddressTypeId, Name, Description) VALUES
(1, 'Home', 'Primary residential address'),
(2, 'Work', 'Business/work address'),
(3, 'Billing', 'Billing and statements address'),
(4, 'Shipping', 'Default shipping address'),
(5, 'Warehouse', 'Supplier warehouse location'),
(6, 'Headquarters', 'Company headquarters');

-- Roles (for auth service)
INSERT INTO auth.Roles (RoleId, Name, Description) VALUES
(1, 'Cashier', 'Can process sales and returns'),
(2, 'Supervisor', 'Can override transactions and manage registers'),
(3, 'Manager', 'Full store operations access'),
(4, 'Admin', 'System administration access'),
(5, 'Inventory', 'Inventory management access'),
(6, 'Reporting', 'Reporting and analytics access');

-- Payment types (for payment service)
INSERT INTO payment.PaymentTypes (PaymentTypeId, Name, Description) VALUES
(1, 'Cash', 'Physical currency payment'),
(2, 'Credit Card', 'Payment via credit card'),
(3, 'Debit Card', 'Payment via debit card'),
(4, 'Mobile Payment', 'Payment via mobile wallet'),
(5, 'Gift Card', 'Payment via store gift card'),
(6, 'Store Credit', 'Payment using customer store credit'),
(7, 'Bank Transfer', 'Direct bank transfer payment'),
(8, 'Crypto', 'Cryptocurrency payment');

-- Discount types (for promotions service)
INSERT INTO promotion.DiscountTypes (DiscountTypeId, Name, Description) VALUES
(1, 'Percentage', 'Percentage discount'),
(2, 'Fixed Amount', 'Fixed monetary amount discount'),
(3, 'BOGO', 'Buy One Get One free/discounted'),
(4, 'Bundle', 'Product bundle discount'),
(5, 'Loyalty', 'Loyalty program discount'),
(6, 'Seasonal', 'Seasonal promotion discount');

/******************** INDEX CREATION ********************/
-- Identity Service
CREATE INDEX IX_Users_Email ON auth.Users(Email);
CREATE INDEX IX_RegistrationTokens_Expiration ON auth.RegistrationTokens(Expiration) WHERE IsUsed = 0;

-- Store Service
CREATE INDEX IX_Registers_Store ON store.Registers(StoreId);
CREATE INDEX IX_Shifts_Employee ON store.Shifts(EmployeeId);

-- Product Service
CREATE INDEX IX_Products_SKU ON catalog.Products(SKU);
CREATE INDEX IX_Products_Category ON catalog.Products(CategoryId);
CREATE INDEX IX_CountryPricing_Product ON catalog.CountryPricing(ProductId);

-- Inventory Service
CREATE INDEX IX_InventoryItems_StoreProduct ON inventory.InventoryItems(StoreId, ProductId);
CREATE INDEX IX_StockMovements_Date ON inventory.StockMovements(MovementDate);

-- Supplier Service
CREATE INDEX IX_SupplierContacts_Supplier ON supplier.SupplierContacts(SupplierId);
CREATE INDEX IX_SupplierAddresses_Supplier ON supplier.SupplierAddresses(SupplierId);

-- Promotion Service
CREATE INDEX IX_DiscountCampaigns_Active ON promotion.DiscountCampaigns(IsActive) WHERE IsActive = 1;
CREATE INDEX IX_Promotions_DateRange ON promotion.Promotions(StartDate, EndDate);

-- Customer Service
CREATE INDEX IX_Customers_Membership ON customer.Customers(MembershipNumber);
CREATE INDEX IX_CustomerContactNumbers_Customer ON customer.CustomerContactNumbers(CustomerId);
CREATE INDEX IX_CustomerAddresses_Customer ON customer.CustomerAddresses(CustomerId);
CREATE INDEX IX_GiftCards_Active ON customer.GiftCards(IsActive) WHERE IsActive = 1;

-- Transaction Service
CREATE INDEX IX_Sales_DateStore ON sales.Sales(TransactionTime, StoreId);
CREATE INDEX IX_SaleDetails_Sale ON sales.SaleDetails(SaleId);

-- Payment Service
CREATE INDEX IX_SalePayments_Sale ON payment.SalePayments(SaleId);
CREATE INDEX IX_PaymentDetails_Unsettled ON payment.PaymentDetails(IsSettled) WHERE IsSettled = 0;

-- Reporting Service
CREATE INDEX IX_SalesSnapshots_Date ON reporting.SalesSnapshots(SaleDate);
CREATE INDEX IX_InventorySnapshots_Date ON reporting.InventorySnapshots(SnapshotDate);
CREATE INDEX IX_InventorySnapshots_Product ON reporting.InventorySnapshots(ProductId);

PRINT 'Database schema created successfully with all domains and enhanced contact management';