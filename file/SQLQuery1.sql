-- ============================================================
-- SchoolDeviceStoreDB - Database schema from ERD
-- ============================================================

IF DB_ID(N'SchoolDeviceStoreDB') IS NULL
BEGIN
    CREATE DATABASE SchoolDeviceStoreDB;
END
GO

USE SchoolDeviceStoreDB;
GO

-- Drop children first to avoid FK conflicts when rerunning script
IF OBJECT_ID(N'dbo.SalesOrderDetails', N'U') IS NOT NULL DROP TABLE dbo.SalesOrderDetails;
IF OBJECT_ID(N'dbo.PurchaseOrderDetails', N'U') IS NOT NULL DROP TABLE dbo.PurchaseOrderDetails;
IF OBJECT_ID(N'dbo.InventoryLogs', N'U') IS NOT NULL DROP TABLE dbo.InventoryLogs;
IF OBJECT_ID(N'dbo.AuditLogs', N'U') IS NOT NULL DROP TABLE dbo.AuditLogs;
IF OBJECT_ID(N'dbo.SalesOrders', N'U') IS NOT NULL DROP TABLE dbo.SalesOrders;
IF OBJECT_ID(N'dbo.PurchaseOrders', N'U') IS NOT NULL DROP TABLE dbo.PurchaseOrders;
IF OBJECT_ID(N'dbo.Promotions', N'U') IS NOT NULL DROP TABLE dbo.Promotions;
IF OBJECT_ID(N'dbo.Products', N'U') IS NOT NULL DROP TABLE dbo.Products;
IF OBJECT_ID(N'dbo.Customers', N'U') IS NOT NULL DROP TABLE dbo.Customers;
IF OBJECT_ID(N'dbo.Manufacturers', N'U') IS NOT NULL DROP TABLE dbo.Manufacturers;
IF OBJECT_ID(N'dbo.Categories', N'U') IS NOT NULL DROP TABLE dbo.Categories;
IF OBJECT_ID(N'dbo.Suppliers', N'U') IS NOT NULL DROP TABLE dbo.Suppliers;
IF OBJECT_ID(N'dbo.Employees', N'U') IS NOT NULL DROP TABLE dbo.Employees;
IF OBJECT_ID(N'dbo.Roles', N'U') IS NOT NULL DROP TABLE dbo.Roles;
IF OBJECT_ID(N'dbo.Settings', N'U') IS NOT NULL DROP TABLE dbo.Settings;
GO

-- 1) Roles
CREATE TABLE dbo.Roles (
    RoleId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    RoleName NVARCHAR(50) NOT NULL UNIQUE
);
GO

-- 2) Employees
CREATE TABLE dbo.Employees (
    EmployeeId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL UNIQUE,
    PasswordHash VARBINARY(256) NOT NULL,
    PasswordSalt VARBINARY(128) NOT NULL,
    FullName NVARCHAR(150) NOT NULL,
    Email NVARCHAR(150) NULL,
    Phone NVARCHAR(50) NULL,
    RoleId INT NOT NULL,
    CreatedAt DATETIME NOT NULL CONSTRAINT DF_Employees_CreatedAt DEFAULT (GETDATE()),
    IsActive BIT NOT NULL CONSTRAINT DF_Employees_IsActive DEFAULT (1),
    CONSTRAINT FK_Employees_Roles FOREIGN KEY (RoleId) REFERENCES dbo.Roles(RoleId)
);
GO

-- 3) Core catalog tables
CREATE TABLE dbo.Categories (
    CategoryId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    CategoryName NVARCHAR(150) NOT NULL,
    Description NVARCHAR(500) NULL
);
GO

CREATE TABLE dbo.Suppliers (
    SupplierId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    SupplierName NVARCHAR(200) NOT NULL,
    ContactName NVARCHAR(150) NULL,
    Phone NVARCHAR(50) NULL,
    Email NVARCHAR(150) NULL,
    Address NVARCHAR(300) NULL
);
GO

CREATE TABLE dbo.Manufacturers (
    ManufacturerId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    ManufacturerName NVARCHAR(200) NOT NULL,
    Description NVARCHAR(500) NULL
);
GO

CREATE TABLE dbo.Customers (
    CustomerId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    CustomerCode NVARCHAR(50) NOT NULL UNIQUE,
    FullName NVARCHAR(200) NOT NULL,
    Email NVARCHAR(150) NULL,
    Phone NVARCHAR(50) NULL,
    Address NVARCHAR(300) NULL,
    LoyaltyPoints INT NOT NULL CONSTRAINT DF_Customers_LoyaltyPoints DEFAULT (0),
    CreatedAt DATETIME NOT NULL CONSTRAINT DF_Customers_CreatedAt DEFAULT (GETDATE())
);
GO

-- 4) Products
CREATE TABLE dbo.Products (
    ProductId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    ProductCode NVARCHAR(50) NOT NULL UNIQUE,
    ProductName NVARCHAR(250) NOT NULL,
    CategoryId INT NULL,
    ManufacturerId INT NULL,
    SupplierId INT NULL,
    Quantity INT NOT NULL CONSTRAINT DF_Products_Quantity DEFAULT (0),
    UnitPrice DECIMAL(18,2) NOT NULL CONSTRAINT DF_Products_UnitPrice DEFAULT (0),
    PurchasePrice DECIMAL(18,2) NOT NULL CONSTRAINT DF_Products_PurchasePrice DEFAULT (0),
    ImagePath NVARCHAR(500) NULL,
    Description NVARCHAR(1000) NULL,
    Status NVARCHAR(50) NOT NULL CONSTRAINT DF_Products_Status DEFAULT (N'Available'),
    CONSTRAINT FK_Products_Categories FOREIGN KEY (CategoryId) REFERENCES dbo.Categories(CategoryId),
    CONSTRAINT FK_Products_Manufacturers FOREIGN KEY (ManufacturerId) REFERENCES dbo.Manufacturers(ManufacturerId),
    CONSTRAINT FK_Products_Suppliers FOREIGN KEY (SupplierId) REFERENCES dbo.Suppliers(SupplierId),
    CONSTRAINT CK_Products_Quantity_NonNegative CHECK (Quantity >= 0),
    CONSTRAINT CK_Products_UnitPrice_NonNegative CHECK (UnitPrice >= 0),
    CONSTRAINT CK_Products_PurchasePrice_NonNegative CHECK (PurchasePrice >= 0)
);
GO

-- 5) Promotions
CREATE TABLE dbo.Promotions (
    PromotionId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    PromotionCode NVARCHAR(50) NOT NULL UNIQUE,
    PromotionName NVARCHAR(200) NOT NULL,
    Description NVARCHAR(500) NULL,
    DiscountType NVARCHAR(50) NOT NULL,
    DiscountValue DECIMAL(18,2) NOT NULL,
    MinOrderAmount DECIMAL(18,2) NULL,
    MaxDiscountAmount DECIMAL(18,2) NULL,
    StartDate DATETIME NOT NULL,
    EndDate DATETIME NOT NULL,
    UsageLimit INT NULL,
    UsageCount INT NOT NULL CONSTRAINT DF_Promotions_UsageCount DEFAULT (0),
    IsActive BIT NOT NULL CONSTRAINT DF_Promotions_IsActive DEFAULT (1),
    AppliesTo NVARCHAR(50) NULL,
    ApplyTargetId INT NULL,
    CreatedBy INT NULL,
    CreatedAt DATETIME NOT NULL CONSTRAINT DF_Promotions_CreatedAt DEFAULT (GETDATE()),
    CONSTRAINT FK_Promotions_Employees FOREIGN KEY (CreatedBy) REFERENCES dbo.Employees(EmployeeId),
    CONSTRAINT CK_Promotions_DateRange CHECK (EndDate >= StartDate)
);
GO

-- 6) Purchase orders
CREATE TABLE dbo.PurchaseOrders (
    PurchaseOrderId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    SupplierId INT NOT NULL,
    CreatedBy INT NOT NULL,
    OrderDate DATETIME NOT NULL CONSTRAINT DF_PurchaseOrders_OrderDate DEFAULT (GETDATE()),
    TotalAmount DECIMAL(18,2) NOT NULL CONSTRAINT DF_PurchaseOrders_TotalAmount DEFAULT (0),
    CONSTRAINT FK_PurchaseOrders_Suppliers FOREIGN KEY (SupplierId) REFERENCES dbo.Suppliers(SupplierId),
    CONSTRAINT FK_PurchaseOrders_Employees FOREIGN KEY (CreatedBy) REFERENCES dbo.Employees(EmployeeId),
    CONSTRAINT CK_PurchaseOrders_TotalAmount_NonNegative CHECK (TotalAmount >= 0)
);
GO

CREATE TABLE dbo.PurchaseOrderDetails (
    PurchaseOrderDetailId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    PurchaseOrderId INT NOT NULL,
    ProductId INT NOT NULL,
    Quantity INT NOT NULL,
    UnitPrice DECIMAL(18,2) NOT NULL,
    CONSTRAINT FK_POD_PurchaseOrders FOREIGN KEY (PurchaseOrderId) REFERENCES dbo.PurchaseOrders(PurchaseOrderId),
    CONSTRAINT FK_POD_Products FOREIGN KEY (ProductId) REFERENCES dbo.Products(ProductId),
    CONSTRAINT CK_PurchaseOrderDetails_Quantity_Positive CHECK (Quantity > 0),
    CONSTRAINT CK_PurchaseOrderDetails_UnitPrice_NonNegative CHECK (UnitPrice >= 0)
);
GO

-- 7) Sales orders
CREATE TABLE dbo.SalesOrders (
    SalesOrderId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    SalesOrderCode NVARCHAR(50) NOT NULL UNIQUE,
    CustomerId INT NULL,
    CreatedBy INT NOT NULL,
    OrderDate DATETIME NOT NULL CONSTRAINT DF_SalesOrders_OrderDate DEFAULT (GETDATE()),
    SubTotal DECIMAL(18,2) NOT NULL CONSTRAINT DF_SalesOrders_SubTotal DEFAULT (0),
    Discount DECIMAL(18,2) NOT NULL CONSTRAINT DF_SalesOrders_Discount DEFAULT (0),
    VAT DECIMAL(18,2) NOT NULL CONSTRAINT DF_SalesOrders_VAT DEFAULT (0),
    TotalAmount DECIMAL(18,2) NOT NULL CONSTRAINT DF_SalesOrders_TotalAmount DEFAULT (0),
    PromotionId INT NULL,
    CONSTRAINT FK_SalesOrders_Customers FOREIGN KEY (CustomerId) REFERENCES dbo.Customers(CustomerId),
    CONSTRAINT FK_SalesOrders_Employees FOREIGN KEY (CreatedBy) REFERENCES dbo.Employees(EmployeeId),
    CONSTRAINT FK_SalesOrders_Promotions FOREIGN KEY (PromotionId) REFERENCES dbo.Promotions(PromotionId),
    CONSTRAINT CK_SalesOrders_SubTotal_NonNegative CHECK (SubTotal >= 0),
    CONSTRAINT CK_SalesOrders_Discount_NonNegative CHECK (Discount >= 0),
    CONSTRAINT CK_SalesOrders_VAT_NonNegative CHECK (VAT >= 0),
    CONSTRAINT CK_SalesOrders_TotalAmount_NonNegative CHECK (TotalAmount >= 0)
);
GO

CREATE TABLE dbo.SalesOrderDetails (
    SalesOrderDetailId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    SalesOrderId INT NOT NULL,
    ProductId INT NOT NULL,
    Quantity INT NOT NULL,
    UnitPrice DECIMAL(18,2) NOT NULL,
    CONSTRAINT FK_SOD_SalesOrders FOREIGN KEY (SalesOrderId) REFERENCES dbo.SalesOrders(SalesOrderId),
    CONSTRAINT FK_SOD_Products FOREIGN KEY (ProductId) REFERENCES dbo.Products(ProductId),
    CONSTRAINT CK_SalesOrderDetails_Quantity_Positive CHECK (Quantity > 0),
    CONSTRAINT CK_SalesOrderDetails_UnitPrice_NonNegative CHECK (UnitPrice >= 0)
);
GO

-- 8) Logging tables
CREATE TABLE dbo.InventoryLogs (
    InventoryLogId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    ProductId INT NOT NULL,
    Change INT NOT NULL,
    Reason NVARCHAR(200) NULL,
    ChangedBy INT NULL,
    ChangedAt DATETIME NOT NULL CONSTRAINT DF_InventoryLogs_ChangedAt DEFAULT (GETDATE()),
    CONSTRAINT FK_InventoryLogs_Products FOREIGN KEY (ProductId) REFERENCES dbo.Products(ProductId),
    CONSTRAINT FK_InventoryLogs_Employees FOREIGN KEY (ChangedBy) REFERENCES dbo.Employees(EmployeeId)
);
GO

CREATE TABLE dbo.AuditLogs (
    LogId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    UserId INT NULL,
    Action NVARCHAR(200) NOT NULL,
    Details NVARCHAR(1000) NULL,
    CreatedAt DATETIME NOT NULL CONSTRAINT DF_AuditLogs_CreatedAt DEFAULT (GETDATE()),
    CONSTRAINT FK_AuditLogs_Employees FOREIGN KEY (UserId) REFERENCES dbo.Employees(EmployeeId)
);
GO

-- 9) App settings
CREATE TABLE dbo.Settings (
    SettingKey NVARCHAR(100) NOT NULL PRIMARY KEY,
    SettingValue NVARCHAR(1000) NULL
);
GO

-- 10) Indexes
CREATE INDEX IX_Products_ProductCode ON dbo.Products(ProductCode);
CREATE INDEX IX_SalesOrders_OrderDate ON dbo.SalesOrders(OrderDate);
CREATE INDEX IX_PurchaseOrders_OrderDate ON dbo.PurchaseOrders(OrderDate);
CREATE INDEX IX_Promotions_PromotionCode ON dbo.Promotions(PromotionCode);
GO

PRINT N'SchoolDeviceStoreDB schema has been created successfully.';