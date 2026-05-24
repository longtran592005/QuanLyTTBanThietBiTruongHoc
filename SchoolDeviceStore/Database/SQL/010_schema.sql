-- Schema for School Device Store (compatible with SQL Server 2008/2012)
-- Use this script to create the database and tables. DB name: SchoolDeviceStoreDB

IF DB_ID('SchoolDeviceStoreDB') IS NULL
BEGIN
    CREATE DATABASE SchoolDeviceStoreDB;
END
GO

USE SchoolDeviceStoreDB;
GO

-- =============================================================
-- Bang Login (Dang nhap)
-- =============================================================
CREATE TABLE Login (
    LoginId INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL UNIQUE,
    PasswordHash VARBINARY(256) NOT NULL,
    PasswordSalt VARBINARY(128) NOT NULL,
    EmployeeId INT NULL,           -- Lien ket voi nhan vien (se them FK sau khi tao bang Employees)
    LastLoginAt DATETIME NULL,
    IsLocked BIT DEFAULT 0,
    CreatedAt DATETIME DEFAULT GETDATE()
);

-- =============================================================
-- Bang Nhan vien (gop cot VaiTro thay vi tach bang Roles rieng)
-- =============================================================
CREATE TABLE Employees (
    EmployeeId INT IDENTITY(1,1) PRIMARY KEY,
    FullName NVARCHAR(150) NOT NULL,
    Email NVARCHAR(150),
    Phone NVARCHAR(50),
    VaiTro NVARCHAR(50) NOT NULL DEFAULT N'Nhân viên',  -- Vai tro: Admin, Quan ly, Nhan vien...
    CreatedAt DATETIME DEFAULT GETDATE(),
    IsActive BIT DEFAULT 1
);

-- Them FK tu Login -> Employees
ALTER TABLE Login
    ADD CONSTRAINT FK_Login_Employees FOREIGN KEY(EmployeeId) REFERENCES Employees(EmployeeId);

-- =============================================================
-- Bang Loai san pham (Categories) - da co cot MoTa
-- =============================================================
CREATE TABLE Categories (
    CategoryId INT IDENTITY(1,1) PRIMARY KEY,
    CategoryName NVARCHAR(150) NOT NULL,
    Description NVARCHAR(500)         -- Mo ta ve loai san pham
);

-- =============================================================
-- Bang Nha cung cap (bo sung dia chi Cty, dia chi chi nhanh, SDT lien he)
-- =============================================================
CREATE TABLE Suppliers (
    SupplierId INT IDENTITY(1,1) PRIMARY KEY,
    SupplierName NVARCHAR(200) NOT NULL,
    ContactName NVARCHAR(150),
    Phone NVARCHAR(50),                    -- SDT chinh
    ContactPhone NVARCHAR(50),             -- So dien thoai lien he
    Email NVARCHAR(150),
    Address NVARCHAR(300),                 -- Dia chi chung
    CompanyAddress NVARCHAR(300),           -- Dia chi cong ty
    BranchAddress NVARCHAR(300)             -- Dia chi chi nhanh (neu co)
);

-- =============================================================
-- Bang San pham (gop cot HangSanXuat thay vi tach bang Manufacturers rieng)
-- =============================================================
CREATE TABLE Products (
    ProductId INT IDENTITY(1,1) PRIMARY KEY,
    ProductCode NVARCHAR(50) NOT NULL UNIQUE,
    ProductName NVARCHAR(250) NOT NULL,
    CategoryId INT NULL,
    ManufacturerName NVARCHAR(200) NULL,    -- Ten hang san xuat (gop vao thay vi bang rieng)
    SupplierId INT NULL,
    Quantity INT DEFAULT 0,
    UnitPrice DECIMAL(18,2) DEFAULT 0,
    PurchasePrice DECIMAL(18,2) DEFAULT 0,
    ImagePath NVARCHAR(500),
    Description NVARCHAR(1000),
    Status NVARCHAR(50) DEFAULT 'Available',
    CONSTRAINT FK_Products_Categories FOREIGN KEY(CategoryId) REFERENCES Categories(CategoryId),
    CONSTRAINT FK_Products_Suppliers FOREIGN KEY(SupplierId) REFERENCES Suppliers(SupplierId)
);

-- =============================================================
-- Bang Khach hang
-- =============================================================
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

-- =============================================================
-- Bang Don nhap hang va chi tiet
-- =============================================================
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

-- =============================================================
-- Bang Hoa don ban va chi tiet
-- =============================================================
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

-- =============================================================
-- Bang log ton kho
-- =============================================================
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

CREATE INDEX IX_InventoryLogs_ProductId_ChangedAt ON InventoryLogs(ProductId, ChangedAt DESC);

-- =============================================================
-- Bang log he thong
-- =============================================================
CREATE TABLE AuditLogs (
    LogId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NULL,
    Action NVARCHAR(200) NOT NULL,
    Details NVARCHAR(1000),
    CreatedAt DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_AuditLogs_Employees FOREIGN KEY(UserId) REFERENCES Employees(EmployeeId)
);

-- =============================================================
-- Cai dat he thong
-- =============================================================
CREATE TABLE Settings (
    SettingKey NVARCHAR(100) PRIMARY KEY,
    SettingValue NVARCHAR(1000)
);

-- Indexes
CREATE INDEX IX_Products_ProductCode ON Products(ProductCode);
CREATE INDEX IX_SalesOrders_OrderDate ON SalesOrders(OrderDate);
CREATE INDEX IX_PurchaseOrders_OrderDate ON PurchaseOrders(OrderDate);

GO
