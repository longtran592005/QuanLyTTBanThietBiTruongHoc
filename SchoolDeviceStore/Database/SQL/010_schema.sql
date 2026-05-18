-- Schema for School Device Store (compatible with SQL Server 2008/2012)
-- Use this script to create the database and tables. DB name: SchoolDeviceStoreDB

IF DB_ID('SchoolDeviceStoreDB') IS NULL
BEGIN
    CREATE DATABASE SchoolDeviceStoreDB;
END
GO

USE SchoolDeviceStoreDB;
GO

-- Users / Employees (with roles)
CREATE TABLE Roles (
    RoleId INT IDENTITY(1,1) PRIMARY KEY,
    RoleName NVARCHAR(50) NOT NULL UNIQUE
);

CREATE TABLE Employees (
    EmployeeId INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL UNIQUE,
    PasswordHash VARBINARY(256) NOT NULL,
    PasswordSalt VARBINARY(128) NOT NULL,
    FullName NVARCHAR(150) NOT NULL,
    Email NVARCHAR(150),
    Phone NVARCHAR(50),
    RoleId INT NOT NULL,
    CreatedAt DATETIME DEFAULT GETDATE(),
    IsActive BIT DEFAULT 1,
    CONSTRAINT FK_Employees_Roles FOREIGN KEY(RoleId) REFERENCES Roles(RoleId)
);

-- Product categories, suppliers, manufacturers
CREATE TABLE Categories (
    CategoryId INT IDENTITY(1,1) PRIMARY KEY,
    CategoryName NVARCHAR(150) NOT NULL,
    Description NVARCHAR(500)
);

CREATE TABLE Suppliers (
    SupplierId INT IDENTITY(1,1) PRIMARY KEY,
    SupplierName NVARCHAR(200) NOT NULL,
    ContactName NVARCHAR(150),
    Phone NVARCHAR(50),
    Email NVARCHAR(150),
    Address NVARCHAR(300)
);

CREATE TABLE Manufacturers (
    ManufacturerId INT IDENTITY(1,1) PRIMARY KEY,
    ManufacturerName NVARCHAR(200) NOT NULL,
    Description NVARCHAR(500)
);

-- Products
CREATE TABLE Products (
    ProductId INT IDENTITY(1,1) PRIMARY KEY,
    ProductCode NVARCHAR(50) NOT NULL UNIQUE,
    ProductName NVARCHAR(250) NOT NULL,
    CategoryId INT NULL,
    ManufacturerId INT NULL,
    SupplierId INT NULL,
    Quantity INT DEFAULT 0,
    UnitPrice DECIMAL(18,2) DEFAULT 0,
    PurchasePrice DECIMAL(18,2) DEFAULT 0,
    ImagePath NVARCHAR(500),
    Description NVARCHAR(1000),
    Status NVARCHAR(50) DEFAULT 'Available',
    CONSTRAINT FK_Products_Categories FOREIGN KEY(CategoryId) REFERENCES Categories(CategoryId),
    CONSTRAINT FK_Products_Manufacturers FOREIGN KEY(ManufacturerId) REFERENCES Manufacturers(ManufacturerId),
    CONSTRAINT FK_Products_Suppliers FOREIGN KEY(SupplierId) REFERENCES Suppliers(SupplierId)
);

-- Customers
CREATE TABLE Customers (
    CustomerId INT IDENTITY(1,1) PRIMARY KEY,
    CustomerCode NVARCHAR(50) NOT NULL UNIQUE,
    FullName NVARCHAR(200) NOT NULL,
    Email NVARCHAR(150),
    Phone NVARCHAR(50),
    Address NVARCHAR(300),
    LoyaltyPoints INT DEFAULT 0,
    CreatedAt DATETIME DEFAULT GETDATE()
);

-- Purchases (import) and purchase details
CREATE TABLE PurchaseOrders (
    PurchaseOrderId INT IDENTITY(1,1) PRIMARY KEY,
    SupplierId INT NOT NULL,
    CreatedBy INT NOT NULL,
    OrderDate DATETIME DEFAULT GETDATE(),
    TotalAmount DECIMAL(18,2) DEFAULT 0,
    CONSTRAINT FK_PurchaseOrders_Suppliers FOREIGN KEY(SupplierId) REFERENCES Suppliers(SupplierId),
    CONSTRAINT FK_PurchaseOrders_Employees FOREIGN KEY(CreatedBy) REFERENCES Employees(EmployeeId)
);

CREATE TABLE PurchaseOrderDetails (
    PurchaseOrderDetailId INT IDENTITY(1,1) PRIMARY KEY,
    PurchaseOrderId INT NOT NULL,
    ProductId INT NOT NULL,
    Quantity INT NOT NULL,
    UnitPrice DECIMAL(18,2) NOT NULL,
    CONSTRAINT FK_POD_PurchaseOrders FOREIGN KEY(PurchaseOrderId) REFERENCES PurchaseOrders(PurchaseOrderId),
    CONSTRAINT FK_POD_Products FOREIGN KEY(ProductId) REFERENCES Products(ProductId)
);

-- Sales (invoices) and invoice details
CREATE TABLE SalesOrders (
    SalesOrderId INT IDENTITY(1,1) PRIMARY KEY,
    SalesOrderCode NVARCHAR(50) NOT NULL UNIQUE,
    CustomerId INT NULL,
    CreatedBy INT NOT NULL,
    OrderDate DATETIME DEFAULT GETDATE(),
    SubTotal DECIMAL(18,2) DEFAULT 0,
    Discount DECIMAL(18,2) DEFAULT 0,
    VAT DECIMAL(18,2) DEFAULT 0,
    TotalAmount DECIMAL(18,2) DEFAULT 0,
    CONSTRAINT FK_SalesOrders_Customers FOREIGN KEY(CustomerId) REFERENCES Customers(CustomerId),
    CONSTRAINT FK_SalesOrders_Employees FOREIGN KEY(CreatedBy) REFERENCES Employees(EmployeeId)
);

CREATE TABLE SalesOrderDetails (
    SalesOrderDetailId INT IDENTITY(1,1) PRIMARY KEY,
    SalesOrderId INT NOT NULL,
    ProductId INT NOT NULL,
    Quantity INT NOT NULL,
    UnitPrice DECIMAL(18,2) NOT NULL,
    CONSTRAINT FK_SOD_SalesOrders FOREIGN KEY(SalesOrderId) REFERENCES SalesOrders(SalesOrderId),
    CONSTRAINT FK_SOD_Products FOREIGN KEY(ProductId) REFERENCES Products(ProductId)
);

-- Inventory logs (for tracking stock changes)
CREATE TABLE InventoryLogs (
    InventoryLogId INT IDENTITY(1,1) PRIMARY KEY,
    ProductId INT NOT NULL,
    Change INT NOT NULL,
    Reason NVARCHAR(200),
    ChangedBy INT NULL,
    ChangedAt DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_InventoryLogs_Products FOREIGN KEY(ProductId) REFERENCES Products(ProductId),
    CONSTRAINT FK_InventoryLogs_Employees FOREIGN KEY(ChangedBy) REFERENCES Employees(EmployeeId)
);

-- Audit / System logs
CREATE TABLE AuditLogs (
    LogId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NULL,
    Action NVARCHAR(200) NOT NULL,
    Details NVARCHAR(1000),
    CreatedAt DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_AuditLogs_Employees FOREIGN KEY(UserId) REFERENCES Employees(EmployeeId)
);

-- System settings
CREATE TABLE Settings (
    SettingKey NVARCHAR(100) PRIMARY KEY,
    SettingValue NVARCHAR(1000)
);

-- Indexes for key queries
CREATE INDEX IX_Products_ProductCode ON Products(ProductCode);
CREATE INDEX IX_SalesOrders_OrderDate ON SalesOrders(OrderDate);
CREATE INDEX IX_PurchaseOrders_OrderDate ON PurchaseOrders(OrderDate);

GO

-- Seed default roles
INSERT INTO Roles (RoleName) VALUES ('Admin'),('Manager'),('Staff');
GO

-- Note: Employee admin user will be added in sample data script
