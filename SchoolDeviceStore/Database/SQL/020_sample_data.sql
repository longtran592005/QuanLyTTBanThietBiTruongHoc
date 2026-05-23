-- Sample data for demo
USE SchoolDeviceStoreDB;
GO

-- Them nhan vien admin
INSERT INTO Employees (FullName, Email, Phone, VaiTro)
VALUES (N'System Administrator', 'admin@example.com', '0123456789', N'Admin');

-- Them tai khoan login cho admin (password placeholder, se hash trong code)
INSERT INTO Login (Username, PasswordHash, PasswordSalt, EmployeeId)
VALUES ('admin', 0x00, 0x00, 1);

-- Categories
INSERT INTO Categories (CategoryName, Description) VALUES
(N'Máy chiếu', N'Thiết bị trình chiếu'),
(N'Loa', N'Thiết bị âm thanh'),
(N'Bảng điện tử', N'Bảng tương tác'),
(N'Máy tính', N'PC/Laptop'),
(N'Thiết bị STEM', N'Thiết bị giáo dục STEM'),
(N'Thiết bị thí nghiệm', N'Dụng cụ thí nghiệm');

-- Suppliers (bo sung dia chi cong ty, chi nhanh, SDT lien he)
INSERT INTO Suppliers (SupplierName, ContactName, Phone, ContactPhone, Email, Address, CompanyAddress, BranchAddress) VALUES
(N'Supplier One', N'Contact A', '0900000001', '0900000011', 'sup1@example.com', N'Hanoi', N'123 Tran Duy Hung, Cau Giay, Hanoi', NULL),
(N'Supplier Two', N'Contact B', '0900000002', '0900000022', 'sup2@example.com', N'HCMC', N'456 Nguyen Van Linh, Quan 7, HCMC', N'789 Le Loi, Quan 1, HCMC');

-- Products (ManufacturerName thay vi ManufacturerId)
INSERT INTO Products (ProductCode, ProductName, CategoryId, ManufacturerName, SupplierId, Quantity, UnitPrice, PurchasePrice, Description)
VALUES
('PJ001', N'Máy chiếu Panasonic', 1, N'Panasonic', 1, 10, 15000000, 12000000, N'Máy chiếu độ sáng cao'),
('SPK001', N'Loa JBL 5W', 2, N'JBL', 1, 25, 1200000, 900000, N'Loa di động'),
('TB001', N'Bảng điện tử Samsung', 3, N'Samsung', 2, 5, 30000000, 25000000, N'Bảng tương tác 65 inch');

-- Customers
INSERT INTO Customers (CustomerCode, FullName, Email, Phone, Address) VALUES
('CUST001', N'Trường Tiểu học A', 'truongA@example.com', '0911111111', N'Address A'),
('CUST002', N'Trường THCS B', 'truongB@example.com', '0922222222', N'Address B');

GO

-- Sample purchases and sales will be generated via application demo or additional scripts
