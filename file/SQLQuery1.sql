-- ==================================================================================
-- DỰ ÁN: SCHOOL DEVICE STORE - DESKTOP MANAGEMENT SYSTEM
-- KỊCH BẢN THIẾT LẬP CƠ SỞ DỮ LIỆU ĐẦY ĐỦ CHO SQL SERVER (SCHEMA + DỮ LIỆU MẪU CHI TIẾT)
-- ==================================================================================

-- 1. TẠO CƠ SỞ DỮ LIỆU
IF DB_ID('SchoolDeviceStoreDB') IS NULL
BEGIN
    CREATE DATABASE SchoolDeviceStoreDB;
END
GO

USE SchoolDeviceStoreDB;
GO

-- 2. XÓA BẢNG CŨ NẾU ĐÃ TỒN TẠI (ĐỂ TRÁNH XUNG ĐỘT KHI CHẠY LẠI)
IF OBJECT_ID('dbo.Settings', 'U') IS NOT NULL DROP TABLE dbo.Settings;
IF OBJECT_ID('dbo.AuditLogs', 'U') IS NOT NULL DROP TABLE dbo.AuditLogs;
IF OBJECT_ID('dbo.InventoryLogs', 'U') IS NOT NULL DROP TABLE dbo.InventoryLogs;
IF OBJECT_ID('dbo.SalesOrderDetails', 'U') IS NOT NULL DROP TABLE dbo.SalesOrderDetails;
IF OBJECT_ID('dbo.SalesOrders', 'U') IS NOT NULL DROP TABLE dbo.SalesOrders;
IF OBJECT_ID('dbo.PurchaseOrderDetails', 'U') IS NOT NULL DROP TABLE dbo.PurchaseOrderDetails;
IF OBJECT_ID('dbo.PurchaseOrders', 'U') IS NOT NULL DROP TABLE dbo.PurchaseOrders;
IF OBJECT_ID('dbo.Customers', 'U') IS NOT NULL DROP TABLE dbo.Customers;
IF OBJECT_ID('dbo.Products', 'U') IS NOT NULL DROP TABLE dbo.Products;
IF OBJECT_ID('dbo.Manufacturers', 'U') IS NOT NULL DROP TABLE dbo.Manufacturers;
IF OBJECT_ID('dbo.Suppliers', 'U') IS NOT NULL DROP TABLE dbo.Suppliers;
IF OBJECT_ID('dbo.Categories', 'U') IS NOT NULL DROP TABLE dbo.Categories;
IF OBJECT_ID('dbo.Employees', 'U') IS NOT NULL DROP TABLE dbo.Employees;
IF OBJECT_ID('dbo.Roles', 'U') IS NOT NULL DROP TABLE dbo.Roles;
GO

-- ==================================================================================
-- 3. KHỞI TẠO CẤU TRÚC BẢNG (SCHEMA)
-- ==================================================================================

-- 3.1. Bảng Vai Trò (Roles)
CREATE TABLE Roles (
    RoleId INT IDENTITY(1,1) PRIMARY KEY,
    RoleName NVARCHAR(50) NOT NULL UNIQUE
);

-- 3.2. Bảng Nhân Viên (Employees)
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

-- 3.3. Bảng Danh Mục Sản Phẩm (Categories)
CREATE TABLE Categories (
    CategoryId INT IDENTITY(1,1) PRIMARY KEY,
    CategoryName NVARCHAR(150) NOT NULL,
    Description NVARCHAR(500)
);

-- 3.4. Bảng Nhà Cung Cấp (Suppliers)
CREATE TABLE Suppliers (
    SupplierId INT IDENTITY(1,1) PRIMARY KEY,
    SupplierName NVARCHAR(200) NOT NULL,
    ContactName NVARCHAR(150),
    Phone NVARCHAR(50),
    Email NVARCHAR(150),
    Address NVARCHAR(300)
);

-- 3.5. Bảng Hãng Sản Xuất (Manufacturers)
CREATE TABLE Manufacturers (
    ManufacturerId INT IDENTITY(1,1) PRIMARY KEY,
    ManufacturerName NVARCHAR(200) NOT NULL,
    Description NVARCHAR(500)
);

-- 3.6. Bảng Thiết Bị/Sản Phẩm (Products)
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

-- 3.7. Bảng Khách Hàng (Customers)
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

-- 3.8. Bảng Đơn Nhập Hàng (PurchaseOrders)
CREATE TABLE PurchaseOrders (
    PurchaseOrderId INT IDENTITY(1,1) PRIMARY KEY,
    SupplierId INT NOT NULL,
    CreatedBy INT NOT NULL,
    OrderDate DATETIME DEFAULT GETDATE(),
    TotalAmount DECIMAL(18,2) DEFAULT 0,
    CONSTRAINT FK_PurchaseOrders_Suppliers FOREIGN KEY(SupplierId) REFERENCES Suppliers(SupplierId),
    CONSTRAINT FK_PurchaseOrders_Employees FOREIGN KEY(CreatedBy) REFERENCES Employees(EmployeeId)
);

-- 3.9. Chi Tiết Đơn Nhập Hàng (PurchaseOrderDetails)
CREATE TABLE PurchaseOrderDetails (
    PurchaseOrderDetailId INT IDENTITY(1,1) PRIMARY KEY,
    PurchaseOrderId INT NOT NULL,
    ProductId INT NOT NULL,
    Quantity INT NOT NULL,
    UnitPrice DECIMAL(18,2) NOT NULL,
    CONSTRAINT FK_POD_PurchaseOrders FOREIGN KEY(PurchaseOrderId) REFERENCES PurchaseOrders(PurchaseOrderId),
    CONSTRAINT FK_POD_Products FOREIGN KEY(ProductId) REFERENCES Products(ProductId)
);

-- 3.10. Bảng Hóa Đơn Bán Hàng (SalesOrders)
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

-- 3.11. Chi Tiết Hóa Đơn Bán Hàng (SalesOrderDetails)
CREATE TABLE SalesOrderDetails (
    SalesOrderDetailId INT IDENTITY(1,1) PRIMARY KEY,
    SalesOrderId INT NOT NULL,
    ProductId INT NOT NULL,
    Quantity INT NOT NULL,
    UnitPrice DECIMAL(18,2) NOT NULL,
    CONSTRAINT FK_SOD_SalesOrders FOREIGN KEY(SalesOrderId) REFERENCES SalesOrders(SalesOrderId),
    CONSTRAINT FK_SOD_Products FOREIGN KEY(ProductId) REFERENCES Products(ProductId)
);

-- 3.12. Bảng Theo Dõi Kho (InventoryLogs)
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

-- 3.13. Bảng Nhật Ký Hệ Thống (AuditLogs)
CREATE TABLE AuditLogs (
    LogId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NULL,
    Action NVARCHAR(200) NOT NULL,
    Details NVARCHAR(1000),
    CreatedAt DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_AuditLogs_Employees FOREIGN KEY(UserId) REFERENCES Employees(EmployeeId)
);

-- 3.14. Bảng Cài Đặt Hệ Thống (Settings)
CREATE TABLE Settings (
    SettingKey NVARCHAR(100) PRIMARY KEY,
    SettingValue NVARCHAR(1000)
);

-- 3.15. Thiết lập chỉ mục (Indexes) để tăng hiệu suất truy vấn
CREATE INDEX IX_Products_ProductCode ON Products(ProductCode);
CREATE INDEX IX_SalesOrders_OrderDate ON SalesOrders(OrderDate);
CREATE INDEX IX_PurchaseOrders_OrderDate ON PurchaseOrders(OrderDate);
GO


-- ==================================================================================
-- 4. INSERT DỮ LIỆU MẪU ĐẦY ĐỦ ĐANG CÓ TRONG HỆ THỐNG
-- ==================================================================================

-- 4.1. Nhập bảng vai trò (Roles)
SET IDENTITY_INSERT Roles ON;
INSERT INTO Roles (RoleId, RoleName) VALUES
(1, N'Admin'),
(2, N'Manager'),
(3, N'Staff');
SET IDENTITY_INSERT Roles OFF;
GO

-- 4.2. Nhập bảng nhân viên (Employees)
-- Mật khẩu mặc định là 'admin123', PasswordHash và PasswordSalt được lưu trữ dưới dạng byte nhị phân 0x00 đại diện mẫu.
SET IDENTITY_INSERT Employees ON;
INSERT INTO Employees (EmployeeId, Username, PasswordHash, PasswordSalt, FullName, Email, Phone, RoleId, CreatedAt, IsActive) VALUES
(1, N'admin', 0x00, 0x00, N'System Administrator', N'admin@example.com', N'0123456789', 1, GETDATE(), 1);
SET IDENTITY_INSERT Employees OFF;
GO

-- 4.3. Nhập bảng danh mục sản phẩm (Categories)
SET IDENTITY_INSERT Categories ON;
INSERT INTO Categories (CategoryId, CategoryName, Description) VALUES
(1, N'Máy chiếu', N'Thiết bị trình chiếu'),
(2, N'Loa', N'Thiết bị âm thanh'),
(3, N'Bảng điện tử', N'Bảng tương tác'),
(4, N'Máy tính', N'PC/Laptop'),
(5, N'Thiết bị STEM', N'Thiết bị giáo dục STEM'),
(6, N'Thiết bị thí nghiệm', N'Dụng cụ thí nghiệm'),
(7, N'Bảng thông minh', N'Smart board trí tuệ nhân tạo'),
(8, N'Máy quay video', N'Thiết bị quay phim'),
(9, N'Kính VR', N'Kính thực tế ảo'),
(10, N'Drone', N'Máy bay không người lái');
SET IDENTITY_INSERT Categories OFF;
GO

-- 4.4. Nhập bảng hãng sản xuất (Manufacturers)
SET IDENTITY_INSERT Manufacturers ON;
INSERT INTO Manufacturers (ManufacturerId, ManufacturerName, Description) VALUES
(1, N'Brand A', N'Hãng sản xuất A'),
(2, N'Brand B', N'Hãng sản xuất B'),
(3, N'Brand C', N'Hãng sản xuất C');
SET IDENTITY_INSERT Manufacturers OFF;
GO

-- 4.5. Nhập bảng nhà cung cấp (Suppliers)
SET IDENTITY_INSERT Suppliers ON;
INSERT INTO Suppliers (SupplierId, SupplierName, ContactName, Phone, Email, Address) VALUES
(1, N'Công ty Điện tử Việt', N'Contact A', N'0900000001', N'supplier1@example.com', N'Hà Nội'),
(2, N'Công ty Công nghệ Phương Đông', N'Contact B', N'0900000002', N'supplier2@example.com', N'Hồ Chí Minh'),
(3, N'Công ty Giáo dục Toàn Cầu', N'Contact C', N'0911000003', N'supplier3@example.com', N'Đà Nẵng'),
(4, N'Nhà phân phối Thiết bị Năng lượng', N'Contact D', N'0912000004', N'supplier4@example.com', N'Hà Nội'),
(5, N'Công ty Nhập khẩu Công nghệ', N'Contact E', N'0913000005', N'supplier5@example.com', N'Bình Dương');
SET IDENTITY_INSERT Suppliers OFF;
GO

-- 4.6. Nhập bảng sản phẩm/thiết bị (Products)
SET IDENTITY_INSERT Products ON;
INSERT INTO Products (ProductId, ProductCode, ProductName, CategoryId, ManufacturerId, SupplierId, Quantity, UnitPrice, PurchasePrice, Description, Status) VALUES
(1, N'PJ001', N'Máy chiếu Panasonic PT-RZ770', 1, 1, 1, 10, 15000000.00, 12000000.00, N'Máy chiếu độ sáng cao 7000 lumen', N'Available'),
(2, N'PJ002', N'Máy chiếu EPSON EB-2250U', 1, 2, 2, 8, 16000000.00, 13000000.00, N'Máy chiếu độ phân giải 4K', N'Available'),
(3, N'PJ003', N'Máy chiếu SONY VPL-PHZ10', 1, 1, 1, 5, 18000000.00, 15000000.00, N'Máy chiếu laser cao cấp', N'Available'),
(4, N'SPK001', N'Loa JBL Control 50-1', 2, 1, 1, 25, 1200000.00, 900000.00, N'Loa di động bluetooth 5W', N'Available'),
(5, N'SPK002', N'Loa JBL Control 28-1', 2, 2, 1, 30, 1500000.00, 1100000.00, N'Loa không dây 10W', N'Available'),
(6, N'SPK003', N'Loa Bose Soundbar 500', 2, 3, 2, 12, 8000000.00, 6500000.00, N'Loa soundbar chuyên nghiệp', N'Available'),
(7, N'TB001', N'Bảng điện tử Samsung Flip 65', 3, 3, 2, 5, 30000000.00, 25000000.00, N'Bảng tương tác 65 inch', N'Available'),
(8, N'TB002', N'Bảng điện tử LG 86UL3D', 3, 1, 2, 4, 35000000.00, 29000000.00, N'Bảng tương tác 86 inch 4K', N'Available'),
(9, N'TB003', N'Bảng điện tử Promethean ActivPanel 75', 3, 2, 1, 3, 28000000.00, 23000000.00, N'Bảng tương tác 75 inch', N'Available'),
(10, N'PC001', N'Máy tính HP EliteDesk 800', 4, 1, 1, 15, 25000000.00, 20000000.00, N'Máy tính để bàn i7', N'Available'),
(11, N'PC002', N'Laptop Dell XPS 13', 4, 2, 2, 8, 40000000.00, 32000000.00, N'Laptop siêu mỏng FHD', N'Available'),
(12, N'PC003', N'iMac 27 inch', 4, 3, 2, 3, 60000000.00, 48000000.00, N'Máy tính Mac i9', N'Available'),
(13, N'STM001', N'Robot VEX Robotics Kit', 5, 1, 3, 20, 5000000.00, 4000000.00, N'Bộ kit robot lập trình STEM', N'Available'),
(14, N'STM002', N'Arduino Mega 2560', 5, 2, 3, 40, 500000.00, 350000.00, N'Vi điều khiển Arduino', N'Available'),
(15, N'STM003', N'Micro:bit Bundle', 5, 3, 1, 50, 800000.00, 600000.00, N'Máy tính siêu nhỏ lập trình', N'Available'),
(16, N'EXP001', N'Kính hiển vi kỹ thuật số', 6, 1, 1, 12, 3000000.00, 2400000.00, N'Kính hiển vi USB 1080p', N'Available'),
(17, N'EXP002', N'Bộ thí nghiệm Hóa học', 6, 2, 3, 8, 2000000.00, 1600000.00, N'Bộ 50 thí nghiệm hóa học', N'Available'),
(18, N'EXP003', N'Bộ từ trường và điện', 6, 3, 3, 10, 1500000.00, 1200000.00, N'Bộ từ trường 30 chi tiết', N'Available'),
(19, N'SB001', N'Bảng thông minh Newline 55', 7, 2, 2, 4, 15000000.00, 12000000.00, N'Bảng thông minh 55 inch', N'Available'),
(20, N'SB002', N'Bảng thông minh ViewSonic 65', 7, 3, 2, 3, 18000000.00, 14500000.00, N'Bảng thông minh 65 inch', N'Available'),
(21, N'VID001', N'Máy quay Video Panasonic HC-X2000', 8, 1, 1, 2, 25000000.00, 20000000.00, N'Máy quay 4K chuyên nghiệp', N'Available'),
(22, N'VID002', N'Máy quay GoPro Hero 11', 8, 2, 1, 10, 15000000.00, 12000000.00, N'Camera hành động 4K', N'Available'),
(23, N'VR001', N'Meta Quest 3', 9, 3, 2, 8, 12000000.00, 9500000.00, N'Kính VR 128GB', N'Available'),
(24, N'VR002', N'HTC Vive XR Elite', 9, 1, 2, 4, 28000000.00, 22000000.00, N'Kính VR cao cấp', N'Available'),
(25, N'DRN001', N'DJI Air 3S', 10, 2, 1, 5, 35000000.00, 28000000.00, N'Drone 4K thời lượng pin 46 phút', N'Available'),
(26, N'DRN002', N'DJI Mini 3 Pro', 10, 3, 1, 12, 20000000.00, 16000000.00, N'Drone nhỏ gọn 250g', N'Available'),
(27, N'ACC001', N'Màn hình phụ ASUS PA247CV', 4, 1, 1, 10, 5000000.00, 4000000.00, N'Màn hình 24 inch màu sắc chính xác', N'Available'),
(28, N'ACC002', N'Bàn phím Mechanical RGB', 4, 2, 1, 20, 2500000.00, 1800000.00, N'Bàn phím cơ học LED RGB', N'Available'),
(29, N'ACC003', N'Chuột Gaming Corsair M65', 4, 3, 1, 25, 1500000.00, 1100000.00, N'Chuột chơi game RGB', N'Available');
SET IDENTITY_INSERT Products OFF;
GO

-- 4.7. Nhập bảng khách hàng (Customers)
SET IDENTITY_INSERT Customers ON;
INSERT INTO Customers (CustomerId, CustomerCode, FullName, Email, Phone, Address, LoyaltyPoints, CreatedAt) VALUES
(1, N'CUST001', N'Trường Tiểu học A', N'truongA@example.com', N'0911111111', N'123 Lê Lợi, Hà Nội', 100, GETDATE()),
(2, N'CUST002', N'Trường THCS B', N'truongB@example.com', N'0922222222', N'456 Nguyễn Huệ, HCM', 150, GETDATE()),
(3, N'CUST003', N'Trường THPT C', N'truongC@example.com', N'0933333333', N'789 Hùng Vương, Đà Nẵng', 200, GETDATE()),
(4, N'CUST004', N'Trường Đại học Sư Phạm', N'truongDHSP@example.com', N'0944444444', N'Phạm Văn Đồng, Hà Nội', 50, GETDATE()),
(5, N'CUST005', N'Trung tâm Đào tạo Kỹ năng', N'trungtamDK@example.com', N'0955555555', N'Trần Hưng Đạo, HCM', 0, GETDATE()),
(6, N'CUST006', N'Thư viện Quốc gia', N'thuvienQG@example.com', N'0966666666', N'Lê Thánh Tôn, HCM', 75, GETDATE()),
(7, N'CUST007', N'Bộ GD&ĐT', N'boGD@example.com', N'0977777777', N'35 Đại Cồ Việt, Hà Nội', 300, GETDATE()),
(8, N'CUST008', N'Trường Kinh tế Công nghệ', N'truongKTCN@example.com', N'0988888888', N'Ngô Gia Tự, Hà Nội', 120, GETDATE());
SET IDENTITY_INSERT Customers OFF;
GO

-- 4.8. Nhập bảng hóa đơn bán hàng (SalesOrders)
SET IDENTITY_INSERT SalesOrders ON;
INSERT INTO SalesOrders (SalesOrderId, SalesOrderCode, CustomerId, CreatedBy, OrderDate, SubTotal, Discount, VAT, TotalAmount) VALUES
(1, N'INV-20260401-001', 1, 1, DATEADD(day, -30, GETDATE()), 30000000.00, 1000000.00, 2900000.00, 31900000.00),
(2, N'INV-20260405-002', 2, 1, DATEADD(day, -25, GETDATE()), 45000000.00, 0.00, 4500000.00, 49500000.00),
(3, N'INV-20260410-003', 3, 1, DATEADD(day, -20, GETDATE()), 15000000.00, 500000.00, 1450000.00, 15950000.00),
(4, N'INV-20260415-004', 4, 1, DATEADD(day, -15, GETDATE()), 60000000.00, 2000000.00, 5800000.00, 63800000.00),
(5, N'INV-20260420-005', 5, 1, DATEADD(day, -10, GETDATE()), 12000000.00, 0.00, 1200000.00, 13200000.00),
(6, N'INV-20260425-006', 1, 1, DATEADD(day, -5, GETDATE()), 25000000.00, 1000000.00, 2400000.00, 26400000.00),
(7, N'INV-20260428-007', 2, 1, DATEADD(day, -2, GETDATE()), 35000000.00, 1500000.00, 3350000.00, 36850000.00),
(8, N'INV-20260501-008', 3, 1, DATEADD(day, -1, GETDATE()), 18000000.00, 0.00, 1800000.00, 19800000.00),
(9, N'INV-20260502-009', NULL, 1, GETDATE(), 5000000.00, 0.00, 500000.00, 5500000.00);
SET IDENTITY_INSERT SalesOrders OFF;
GO

-- 4.9. Nhập bảng chi tiết hóa đơn (SalesOrderDetails)
SET IDENTITY_INSERT SalesOrderDetails ON;
INSERT INTO SalesOrderDetails (SalesOrderDetailId, SalesOrderId, ProductId, Quantity, UnitPrice) VALUES
(1, 1, 1, 2, 15000000.00),
(2, 2, 7, 1, 30000000.00),
(3, 2, 2, 1, 15000000.00),
(4, 3, 4, 10, 1500000.00),
(5, 4, 12, 1, 60000000.00),
(6, 5, 23, 1, 12000000.00),
(7, 6, 10, 1, 25000000.00),
(8, 7, 8, 1, 35000000.00),
(9, 8, 3, 1, 18000000.00),
(10, 9, 13, 1, 5000000.00);
SET IDENTITY_INSERT SalesOrderDetails OFF;
GO

-- 4.10. Nhập bảng cài đặt (Settings)
INSERT INTO Settings (SettingKey, SettingValue) VALUES
(N'DefaultVAT', N'10');
GO

PRINT '=======================================================';
PRINT '  CƠ SỞ DỮ LIỆU SQL SERVER ĐÃ ĐƯỢC THIẾT LẬP THÀNH CÔNG ';
PRINT '=======================================================';
