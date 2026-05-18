-- Sample data for demo
USE SchoolDeviceStoreDB;
GO

-- Add an Admin user (password salted & hashed will be replaced by real hash in code; here we add placeholder)
-- For security, actual password hashing should be done in application code. For demo, we will add a default 'admin' with a known hash when implementing DAL.

INSERT INTO Employees (Username, PasswordHash, PasswordSalt, FullName, Email, Phone, RoleId)
VALUES ('admin', 0x00, 0x00, 'System Administrator', 'admin@example.com', '0123456789', 1);

-- Categories
INSERT INTO Categories (CategoryName, Description) VALUES
('Máy chiếu','Thiết bị trình chiếu'),
('Loa','Thiết bị âm thanh'),
('Bảng điện tử','Bảng tương tác'),
('Máy tính','PC/Laptop'),
('Thiết bị STEM','Thiết bị giáo dục STEM'),
('Thiết bị thí nghiệm','Dụng cụ thí nghiệm');

-- Manufacturers
INSERT INTO Manufacturers (ManufacturerName) VALUES ('Brand A'),('Brand B'),('Brand C');

-- Suppliers
INSERT INTO Suppliers (SupplierName, ContactName, Phone, Email, Address) VALUES
('Supplier One','Contact A','0900000001','sup1@example.com','Hanoi'),
('Supplier Two','Contact B','0900000002','sup2@example.com','HCMC');

-- Products (few examples)
INSERT INTO Products (ProductCode, ProductName, CategoryId, ManufacturerId, SupplierId, Quantity, UnitPrice, PurchasePrice, Description)
VALUES
('PJ001','Máy chiếu Panasonic',1,1,1,10,15000000,12000000,'Máy chiếu độ sáng cao'),
('SPK001','Loa JBL 5W',2,2,1,25,1200000,900000,'Loa di động'),
('TB001','Bảng điện tử Samsung',3,3,2,5,30000000,25000000,'Bảng tương tác 65 inch');

-- Customers
INSERT INTO Customers (CustomerCode, FullName, Email, Phone, Address) VALUES
('CUST001','Trường Tiểu học A','truongA@example.com','0911111111','Address A'),
('CUST002','Trường THCS B','truongB@example.com','0922222222','Address B');

GO

-- Sample purchases and sales will be generated via application demo or additional scripts
