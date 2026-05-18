param(
    [string]$Root = (Split-Path -Parent $PSScriptRoot)
)

$dllPath = Join-Path $Root 'DAL\bin\Debug\net48\System.Data.SQLite.dll'
$dalPath = Join-Path $Root 'DAL\bin\Debug\net48\SchoolDeviceStore.DAL.dll'
$dbPath = Join-Path $Root 'GUI.WinForms\bin\Debug\net48\SchoolDeviceStore.db'

if (-not (Test-Path $dllPath)) {
    throw "SQLite assembly not found at $dllPath. Build the solution first."
}

Add-Type -Path $dllPath
Add-Type -Path $dalPath

New-Item -ItemType Directory -Force -Path (Split-Path -Parent $dbPath) | Out-Null

$connection = New-Object System.Data.SQLite.SQLiteConnection("Data Source=$dbPath;Version=3;")
$connection.Open()

if (-not (Test-Path $dbPath)) {
    $statements = @(
    @"
PRAGMA foreign_keys = ON;
"@,
    @"
CREATE TABLE Roles (
    RoleId INTEGER PRIMARY KEY AUTOINCREMENT,
    RoleName TEXT NOT NULL UNIQUE
);
CREATE TABLE Employees (
    EmployeeId INTEGER PRIMARY KEY AUTOINCREMENT,
    Username TEXT NOT NULL UNIQUE,
    PasswordHash BLOB NOT NULL,
    PasswordSalt BLOB NOT NULL,
    FullName TEXT NOT NULL,
    Email TEXT,
    Phone TEXT,
    RoleId INTEGER NOT NULL,
    CreatedAt TEXT DEFAULT CURRENT_TIMESTAMP,
    IsActive INTEGER DEFAULT 1,
    FOREIGN KEY(RoleId) REFERENCES Roles(RoleId)
);
CREATE TABLE Categories (
    CategoryId INTEGER PRIMARY KEY AUTOINCREMENT,
    CategoryName TEXT NOT NULL,
    Description TEXT
);
CREATE TABLE Suppliers (
    SupplierId INTEGER PRIMARY KEY AUTOINCREMENT,
    SupplierName TEXT NOT NULL,
    ContactName TEXT,
    Phone TEXT,
    Email TEXT,
    Address TEXT
);
CREATE TABLE Manufacturers (
    ManufacturerId INTEGER PRIMARY KEY AUTOINCREMENT,
    ManufacturerName TEXT NOT NULL,
    Description TEXT
);
CREATE TABLE Products (
    ProductId INTEGER PRIMARY KEY AUTOINCREMENT,
    ProductCode TEXT NOT NULL UNIQUE,
    ProductName TEXT NOT NULL,
    CategoryId INTEGER NULL,
    ManufacturerId INTEGER NULL,
    SupplierId INTEGER NULL,
    Quantity INTEGER DEFAULT 0,
    UnitPrice NUMERIC DEFAULT 0,
    PurchasePrice NUMERIC DEFAULT 0,
    ImagePath TEXT,
    Description TEXT,
    Status TEXT DEFAULT 'Available',
    FOREIGN KEY(CategoryId) REFERENCES Categories(CategoryId),
    FOREIGN KEY(ManufacturerId) REFERENCES Manufacturers(ManufacturerId),
    FOREIGN KEY(SupplierId) REFERENCES Suppliers(SupplierId)
);
CREATE TABLE Customers (
    CustomerId INTEGER PRIMARY KEY AUTOINCREMENT,
    CustomerCode TEXT NOT NULL UNIQUE,
    FullName TEXT NOT NULL,
    Email TEXT,
    Phone TEXT,
    Address TEXT,
    LoyaltyPoints INTEGER DEFAULT 0,
    CreatedAt TEXT DEFAULT CURRENT_TIMESTAMP
);
CREATE TABLE PurchaseOrders (
    PurchaseOrderId INTEGER PRIMARY KEY AUTOINCREMENT,
    SupplierId INTEGER NOT NULL,
    CreatedBy INTEGER NOT NULL,
    OrderDate TEXT DEFAULT CURRENT_TIMESTAMP,
    TotalAmount NUMERIC DEFAULT 0,
    FOREIGN KEY(SupplierId) REFERENCES Suppliers(SupplierId),
    FOREIGN KEY(CreatedBy) REFERENCES Employees(EmployeeId)
);
CREATE TABLE PurchaseOrderDetails (
    PurchaseOrderDetailId INTEGER PRIMARY KEY AUTOINCREMENT,
    PurchaseOrderId INTEGER NOT NULL,
    ProductId INTEGER NOT NULL,
    Quantity INTEGER NOT NULL,
    UnitPrice NUMERIC NOT NULL,
    FOREIGN KEY(PurchaseOrderId) REFERENCES PurchaseOrders(PurchaseOrderId),
    FOREIGN KEY(ProductId) REFERENCES Products(ProductId)
);
CREATE TABLE SalesOrders (
    SalesOrderId INTEGER PRIMARY KEY AUTOINCREMENT,
    SalesOrderCode TEXT NOT NULL UNIQUE,
    CustomerId INTEGER NULL,
    CreatedBy INTEGER NOT NULL,
    OrderDate TEXT DEFAULT CURRENT_TIMESTAMP,
    SubTotal NUMERIC DEFAULT 0,
    Discount NUMERIC DEFAULT 0,
    VAT NUMERIC DEFAULT 0,
    TotalAmount NUMERIC DEFAULT 0,
    FOREIGN KEY(CustomerId) REFERENCES Customers(CustomerId),
    FOREIGN KEY(CreatedBy) REFERENCES Employees(EmployeeId)
);
CREATE TABLE SalesOrderDetails (
    SalesOrderDetailId INTEGER PRIMARY KEY AUTOINCREMENT,
    SalesOrderId INTEGER NOT NULL,
    ProductId INTEGER NOT NULL,
    Quantity INTEGER NOT NULL,
    UnitPrice NUMERIC NOT NULL,
    FOREIGN KEY(SalesOrderId) REFERENCES SalesOrders(SalesOrderId),
    FOREIGN KEY(ProductId) REFERENCES Products(ProductId)
);
CREATE TABLE InventoryLogs (
    InventoryLogId INTEGER PRIMARY KEY AUTOINCREMENT,
    ProductId INTEGER NOT NULL,
    Change INTEGER NOT NULL,
    Reason TEXT,
    ChangedBy INTEGER NULL,
    ChangedAt TEXT DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY(ProductId) REFERENCES Products(ProductId),
    FOREIGN KEY(ChangedBy) REFERENCES Employees(EmployeeId)
);
CREATE TABLE AuditLogs (
    LogId INTEGER PRIMARY KEY AUTOINCREMENT,
    UserId INTEGER NULL,
    Action TEXT NOT NULL,
    Details TEXT,
    CreatedAt TEXT DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY(UserId) REFERENCES Employees(EmployeeId)
);
CREATE TABLE Settings (
    SettingKey TEXT PRIMARY KEY,
    SettingValue TEXT
);
CREATE INDEX IX_Products_ProductCode ON Products(ProductCode);
CREATE INDEX IX_SalesOrders_OrderDate ON SalesOrders(OrderDate);
CREATE INDEX IX_PurchaseOrders_OrderDate ON PurchaseOrders(OrderDate);
"@,
    @"
INSERT INTO Roles (RoleName) VALUES ('Admin'),('Manager'),('Staff');
INSERT INTO Categories (CategoryName, Description) VALUES
('Máy chiếu','Thiết bị trình chiếu'),
('Loa','Thiết bị âm thanh'),
('Bảng điện tử','Bảng tương tác'),
('Máy tính','PC/Laptop'),
('Thiết bị STEM','Thiết bị giáo dục STEM'),
('Thiết bị thí nghiệm','Dụng cụ thí nghiệm');
INSERT INTO Manufacturers (ManufacturerName, Description) VALUES
('Brand A', NULL),('Brand B', NULL),('Brand C', NULL);
INSERT INTO Suppliers (SupplierName, ContactName, Phone, Email, Address) VALUES
('Supplier One','Contact A','0900000001','sup1@example.com','Hanoi'),
('Supplier Two','Contact B','0900000002','sup2@example.com','HCMC');
INSERT INTO Products (ProductCode, ProductName, CategoryId, ManufacturerId, SupplierId, Quantity, UnitPrice, PurchasePrice, Description)
VALUES
('PJ001','Máy chiếu Panasonic',1,1,1,10,15000000,12000000,'Máy chiếu độ sáng cao'),
('SPK001','Loa JBL 5W',2,2,1,25,1200000,900000,'Loa di động'),
('TB001','Bảng điện tử Samsung',3,3,2,5,30000000,25000000,'Bảng tương tác 65 inch');
INSERT INTO Customers (CustomerCode, FullName, Email, Phone, Address) VALUES
('CUST001','Trường Tiểu học A','truongA@example.com','0911111111','Address A'),
('CUST002','Trường THCS B','truongB@example.com','0922222222','Address B');
INSERT INTO Settings (SettingKey, SettingValue) VALUES ('DefaultVAT','10');
"@
    )

    foreach ($sql in $statements) {
        $cmd = $connection.CreateCommand()
        $cmd.CommandText = $sql
        [void]$cmd.ExecuteNonQuery()
    }
}

$hash = $null
$salt = $null
[DAL.Utils.PasswordHelper]::CreatePasswordHash('admin123', [ref]$hash, [ref]$salt)

$adminCmd = $connection.CreateCommand()
$adminCmd.CommandText = "INSERT OR IGNORE INTO Employees (Username, PasswordHash, PasswordSalt, FullName, Email, Phone, RoleId, IsActive) VALUES (@username, @hash, @salt, @fullname, @email, @phone, @roleId, @isActive);"
[void]$adminCmd.Parameters.AddWithValue('@username', 'admin')
[void]$adminCmd.Parameters.AddWithValue('@hash', $hash)
[void]$adminCmd.Parameters.AddWithValue('@salt', $salt)
[void]$adminCmd.Parameters.AddWithValue('@fullname', 'System Administrator')
[void]$adminCmd.Parameters.AddWithValue('@email', 'admin@example.com')
[void]$adminCmd.Parameters.AddWithValue('@phone', '0123456789')
[void]$adminCmd.Parameters.AddWithValue('@roleId', 1)
[void]$adminCmd.Parameters.AddWithValue('@isActive', 1)
$null = $adminCmd.ExecuteNonQuery()

$connection.Close()
Write-Host "Created SQLite demo database at $dbPath"