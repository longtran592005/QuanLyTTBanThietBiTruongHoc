using System;
using System.Configuration;
using System.Data.SQLite;
using System.Security.Cryptography;

namespace DAL
{
    public static class DemoDatabaseInitializer
    {
        public static void EnsureCreated()
        {
            try
            {
                var connStr = DbHelper.GetConnectionString();
                Console.WriteLine("[DEBUG] Database connection string: " + connStr);
                System.Diagnostics.Debug.WriteLine("[DEBUG] Database connection string: " + connStr);
                
                using (var conn = new SQLiteConnection(connStr))
                {
                    conn.Open();
                    Console.WriteLine("[DEBUG] Database connection opened successfully");

                    ExecuteNonQuery(conn, "PRAGMA foreign_keys = ON;");

                    ExecuteNonQuery(conn, @"
CREATE TABLE IF NOT EXISTS Roles (
    RoleId INTEGER PRIMARY KEY AUTOINCREMENT,
    RoleName TEXT NOT NULL UNIQUE
);

CREATE TABLE IF NOT EXISTS Employees (
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

CREATE TABLE IF NOT EXISTS Categories (
    CategoryId INTEGER PRIMARY KEY AUTOINCREMENT,
    CategoryName TEXT NOT NULL,
    Description TEXT
);

CREATE TABLE IF NOT EXISTS Suppliers (
    SupplierId INTEGER PRIMARY KEY AUTOINCREMENT,
    SupplierName TEXT NOT NULL,
    ContactName TEXT,
    Phone TEXT,
    Email TEXT,
    Address TEXT
);

CREATE TABLE IF NOT EXISTS Products (
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
    FOREIGN KEY(SupplierId) REFERENCES Suppliers(SupplierId)
);

CREATE TABLE IF NOT EXISTS Customers (
    CustomerId INTEGER PRIMARY KEY AUTOINCREMENT,
    CustomerCode TEXT NOT NULL UNIQUE,
    FullName TEXT NOT NULL,
    Email TEXT,
    Phone TEXT,
    Address TEXT,
    LoyaltyPoints INTEGER DEFAULT 0,
    CreatedAt TEXT DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS SalesOrders (
    SalesOrderId INTEGER PRIMARY KEY AUTOINCREMENT,
    SalesOrderCode TEXT NOT NULL UNIQUE,
    CustomerId INTEGER NULL,
    CreatedBy INTEGER NOT NULL,
    OrderDate TEXT DEFAULT CURRENT_TIMESTAMP,
    SubTotal NUMERIC DEFAULT 0,
    Discount NUMERIC DEFAULT 0,
    VAT NUMERIC DEFAULT 0,
    TotalAmount NUMERIC DEFAULT 0,
    PromotionId INTEGER NULL,
    FOREIGN KEY(CustomerId) REFERENCES Customers(CustomerId),
    FOREIGN KEY(CreatedBy) REFERENCES Employees(EmployeeId)
);

CREATE TABLE IF NOT EXISTS SalesOrderDetails (
    SalesOrderDetailId INTEGER PRIMARY KEY AUTOINCREMENT,
    SalesOrderId INTEGER NOT NULL,
    ProductId INTEGER NOT NULL,
    Quantity INTEGER NOT NULL,
    UnitPrice NUMERIC NOT NULL,
    FOREIGN KEY(SalesOrderId) REFERENCES SalesOrders(SalesOrderId),
    FOREIGN KEY(ProductId) REFERENCES Products(ProductId)
);

CREATE TABLE IF NOT EXISTS InventoryLogs (
    InventoryLogId INTEGER PRIMARY KEY AUTOINCREMENT,
    ProductId INTEGER NOT NULL,
    Change INTEGER NOT NULL,
    Reason TEXT,
    ChangedBy INTEGER,
    ChangedAt TEXT DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY(ProductId) REFERENCES Products(ProductId),
    FOREIGN KEY(ChangedBy) REFERENCES Employees(EmployeeId)
);

CREATE TABLE IF NOT EXISTS Promotions (
    PromotionId INTEGER PRIMARY KEY AUTOINCREMENT,
    PromotionCode TEXT NOT NULL UNIQUE,
    PromotionName TEXT NOT NULL,
    Description TEXT,
    DiscountType TEXT NOT NULL DEFAULT 'Percentage',
    DiscountValue NUMERIC NOT NULL DEFAULT 0,
    MinOrderAmount NUMERIC DEFAULT 0,
    MaxDiscountAmount NUMERIC DEFAULT NULL,
    StartDate TEXT NOT NULL,
    EndDate TEXT NOT NULL,
    UsageLimit INTEGER DEFAULT NULL,
    UsageCount INTEGER DEFAULT 0,
    IsActive INTEGER DEFAULT 1,
    AppliesTo TEXT DEFAULT 'All',
    ApplyTargetId INTEGER DEFAULT NULL,
    CreatedBy INTEGER,
    CreatedAt TEXT DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS Settings (
    SettingKey TEXT PRIMARY KEY,
    SettingValue TEXT
);

CREATE INDEX IF NOT EXISTS IX_Products_ProductCode ON Products(ProductCode);
CREATE INDEX IF NOT EXISTS IX_SalesOrders_OrderDate ON SalesOrders(OrderDate);
CREATE INDEX IF NOT EXISTS IX_Promotions_Code ON Promotions(PromotionCode);

-- Seed 5 roles
INSERT OR IGNORE INTO Roles (RoleId, RoleName) VALUES (1, 'Admin');
INSERT OR IGNORE INTO Roles (RoleId, RoleName) VALUES (2, 'Manager');
INSERT OR IGNORE INTO Roles (RoleId, RoleName) VALUES (3, 'Salesperson');
INSERT OR IGNORE INTO Roles (RoleId, RoleName) VALUES (4, 'Warehouse');
INSERT OR IGNORE INTO Roles (RoleId, RoleName) VALUES (5, 'Accountant');

INSERT OR IGNORE INTO Settings (SettingKey, SettingValue) VALUES ('DefaultVAT', '10');
");

                // Auto-migrate schema upgrades for older databases on users' machines
                AddColumnIfMissing(conn, "Employees", "IsActive", "INTEGER DEFAULT 1");
                AddColumnIfMissing(conn, "Employees", "CreatedAt", "TEXT DEFAULT CURRENT_TIMESTAMP");
                AddColumnIfMissing(conn, "SalesOrders", "PromotionId", "INTEGER NULL");

                if (ShouldSeedDemoData())
                {
                    Console.WriteLine("[DEBUG] Seeding demo data...");
                    SeedDemoData(conn);
                    Console.WriteLine("[DEBUG] Demo data seeded successfully");
                }
                else
                {
                    Console.WriteLine("[DEBUG] Demo data seeding is disabled");
                }
                Console.WriteLine("[DEBUG] Database initialization completed successfully");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ERROR] Database initialization failed: " + ex.Message);
                Console.WriteLine("[ERROR] StackTrace: " + ex.StackTrace);
                System.Diagnostics.Debug.WriteLine("[ERROR] Database initialization failed: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("[ERROR] StackTrace: " + ex.StackTrace);
                throw;
            }
        }

        private static bool ShouldSeedDemoData()
        {
            var value = ConfigurationManager.AppSettings["SeedDemoDataOnStartup"];
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            bool parsed;
            if (bool.TryParse(value, out parsed))
            {
                return parsed;
            }

            return false;
        }

        private static void SeedDemoData(SQLiteConnection conn)
        {
            // Check if we need to clear and reseed (e.g., upgrading from old data to new data)
            var productCount = GetCount(conn, "SELECT COUNT(1) FROM Products");
            var categoryCount = GetCount(conn, "SELECT COUNT(1) FROM Categories");
            
            // If we have old data (3 products, 6 categories) but not new data (30 products), clear and reseed
            if (productCount < 20 && categoryCount <= 6)
            {
                Console.WriteLine("[DEBUG] Clearing old demo data and reseeding with new expanded dataset...");
                ExecuteNonQuery(conn, "DELETE FROM Categories; DELETE FROM Suppliers; DELETE FROM Products; DELETE FROM Customers;");
            }

            // Seed Employees with proper password hashes
            SeedEmployees(conn);

            if (GetCount(conn, "SELECT COUNT(1) FROM Categories") == 0)
            {
                ExecuteNonQuery(conn, @"
INSERT INTO Categories (CategoryName, Description) VALUES
('Máy chiếu','Thiết bị trình chiếu'),
('Loa','Thiết bị âm thanh'),
('Bảng điện tử','Bảng tương tác'),
('Máy tính','PC/Laptop'),
('Thiết bị STEM','Thiết bị giáo dục STEM'),
('Thiết bị thí nghiệm','Dụng cụ thí nghiệm'),
('Bảng thông minh','Smart board trí tuệ nhân tạo'),
('Máy quay video','Thiết bị quay phim'),
('Kính VR','Kính thực tế ảo'),
('Drone','Máy bay không người lái');
");
            }

            if (GetCount(conn, "SELECT COUNT(1) FROM Suppliers") == 0)
            {
                ExecuteNonQuery(conn, @"
INSERT INTO Suppliers (SupplierName, ContactName, Phone, Email, Address) VALUES
('Công ty Điện tử Việt','Contact A','0900000001','supplier1@example.com','Hà Nội'),
('Công ty Công nghệ Phương Đông','Contact B','0900000002','supplier2@example.com','Hồ Chí Minh'),
('Công ty Giáo dục Toàn Cầu','Contact C','0911000003','supplier3@example.com','Đà Nẵng'),
('Nhà phân phối Thiết bị Năng lượng','Contact D','0912000004','supplier4@example.com','Hà Nội'),
('Công ty Nhập khẩu Công nghệ','Contact E','0913000005','supplier5@example.com','Bình Dương');
");
            }

            if (GetCount(conn, "SELECT COUNT(1) FROM Products") == 0)
            {
                ExecuteNonQuery(conn, @"
INSERT INTO Products (ProductCode, ProductName, CategoryId, SupplierId, Quantity, UnitPrice, PurchasePrice, Description, Status)
VALUES
('PJ001','Máy chiếu Panasonic PT-RZ770',1,1,10,15000000,12000000,'Máy chiếu độ sáng cao 7000 lumen','Available'),
('PJ002','Máy chiếu EPSON EB-2250U',1,2,8,16000000,13000000,'Máy chiếu độ phân giải 4K','Available'),
('PJ003','Máy chiếu SONY VPL-PHZ10',1,1,5,18000000,15000000,'Máy chiếu laser cao cấp','Available'),
('SPK001','Loa JBL Control 50-1',2,1,25,1200000,900000,'Loa di động bluetooth 5W','Available'),
('SPK002','Loa JBL Control 28-1',2,1,30,1500000,1100000,'Loa không dây 10W','Available'),
('SPK003','Loa Bose Soundbar 500',2,2,12,8000000,6500000,'Loa soundbar chuyên nghiệp','Available'),
('TB001','Bảng điện tử Samsung Flip 65',3,2,5,30000000,25000000,'Bảng tương tác 65 inch','Available'),
('TB002','Bảng điện tử LG 86UL3D',3,2,4,35000000,29000000,'Bảng tương tác 86 inch 4K','Available'),
('TB003','Bảng điện tử Promethean ActivPanel 75',3,1,3,28000000,23000000,'Bảng tương tác 75 inch','Available'),
('PC001','Máy tính HP EliteDesk 800',4,1,15,25000000,20000000,'Máy tính để bàn i7','Available'),
('PC002','Laptop Dell XPS 13',4,2,8,40000000,32000000,'Laptop siêu mỏng FHD','Available'),
('PC003','iMac 27 inch',4,2,3,60000000,48000000,'Máy tính Mac i9','Available'),
('STM001','Robot VEX Robotics Kit',5,3,20,5000000,4000000,'Bộ kit robot lập trình STEM','Available'),
('STM002','Arduino Mega 2560',5,3,40,500000,350000,'Vi điều khiển Arduino','Available'),
('STM003','Micro:bit Bundle',5,1,50,800000,600000,'Máy tính siêu nhỏ lập trình','Available'),
('EXP001','Kính hiển vi kỹ thuật số',6,1,12,3000000,2400000,'Kính hiển vi USB 1080p','Available'),
('EXP002','Bộ thí nghiệm Hóa học',6,3,8,2000000,1600000,'Bộ 50 thí nghiệm hóa học','Available'),
('EXP003','Bộ từ trường và điện',6,3,10,1500000,1200000,'Bộ từ trường 30 chi tiết','Available'),
('SB001','Bảng thông minh Newline 55',7,2,4,15000000,12000000,'Bảng thông minh 55 inch','Available'),
('SB002','Bảng thông minh ViewSonic 65',7,2,3,18000000,14500000,'Bảng thông minh 65 inch','Available'),
('VID001','Máy quay Video Panasonic HC-X2000',8,1,2,25000000,20000000,'Máy quay 4K chuyên nghiệp','Available'),
('VID002','Máy quay GoPro Hero 11',8,1,10,15000000,12000000,'Camera hành động 4K','Available'),
('VR001','Meta Quest 3',9,2,8,12000000,9500000,'Kính VR 128GB','Available'),
('VR002','HTC Vive XR Elite',9,2,4,28000000,22000000,'Kính VR cao cấp','Available'),
('DRN001','DJI Air 3S',10,1,5,35000000,28000000,'Drone 4K thời lượng pin 46 phút','Available'),
('DRN002','DJI Mini 3 Pro',10,1,12,20000000,16000000,'Drone nhỏ gọn 250g','Available'),
('ACC001','Màn hình phụ ASUS PA247CV',4,1,10,5000000,4000000,'Màn hình 24 inch màu sắc chính xác','Available'),
('ACC002','Bàn phím Mechanical RGB',4,1,20,2500000,1800000,'Bàn phím cơ học LED RGB','Available'),
('ACC003','Chuột Gaming Corsair M65',4,1,25,1500000,1100000,'Chuột chơi game RGB','Available');
");
            }

            if (GetCount(conn, "SELECT COUNT(1) FROM Customers") == 0)
            {
                ExecuteNonQuery(conn, @"
INSERT INTO Customers (CustomerCode, FullName, Email, Phone, Address, LoyaltyPoints) VALUES
('CUST001','Trường Tiểu học A','truongA@example.com','0911111111','123 Lê Lợi, Hà Nội',100),
('CUST002','Trường THCS B','truongB@example.com','0922222222','456 Nguyễn Huệ, HCM',150),
('CUST003','Trường THPT C','truongC@example.com','0933333333','789 Hùng Vương, Đà Nẵng',200),
('CUST004','Trường Đại học Sư Phạm','truongDHSP@example.com','0944444444','Phạm Văn Đồng, Hà Nội',50),
('CUST005','Trung tâm Đào tạo Kỹ năng','trungtamDK@example.com','0955555555','Trần Hưng Đạo, HCM',0),
('CUST006','Thư viện Quốc gia','thuvienQG@example.com','0966666666','Lê Thánh Tôn, HCM',75),
('CUST007','Bộ GD&ĐT','boGD@example.com','0977777777','35 Đại Cồ Việt, Hà Nội',300),
('CUST008','Trường Kinh tế Công nghệ','truongKTCN@example.com','0988888888','Ngô Gia Tự, Hà Nội',120);
");
            }

            if (GetCount(conn, "SELECT COUNT(1) FROM SalesOrders") == 0)
            {
                Console.WriteLine("[DEBUG] Seeding SalesOrders and SalesOrderDetails...");
                ExecuteNonQuery(conn, "PRAGMA foreign_keys = OFF;");
                // Create sales orders over the last 30 days, assign to different employees
                ExecuteNonQuery(conn, @"
INSERT INTO SalesOrders (SalesOrderId, SalesOrderCode, CustomerId, CreatedBy, OrderDate, SubTotal, Discount, VAT, TotalAmount) VALUES
(1, 'INV-20260401-001', 1, 1, date('now', '-30 days'), 30000000, 1000000, 2900000, 31900000),
(2, 'INV-20260405-002', 2, 3, date('now', '-25 days'), 45000000, 0, 4500000, 49500000),
(3, 'INV-20260410-003', 3, 3, date('now', '-20 days'), 15000000, 500000, 1450000, 15950000),
(4, 'INV-20260415-004', 4, 4, date('now', '-15 days'), 60000000, 2000000, 5800000, 63800000),
(5, 'INV-20260420-005', 5, 3, date('now', '-10 days'), 12000000, 0, 1200000, 13200000),
(6, 'INV-20260425-006', 1, 7, date('now', '-5 days'), 25000000, 1000000, 2400000, 26400000),
(7, 'INV-20260428-007', 2, 4, date('now', '-2 days'), 35000000, 1500000, 3350000, 36850000),
(8, 'INV-20260501-008', 3, 7, date('now', '-1 days'), 18000000, 0, 1800000, 19800000),
(9, 'INV-20260502-009', NULL, 1, date('now'), 5000000, 0, 500000, 5500000);
");

                ExecuteNonQuery(conn, @"
INSERT INTO SalesOrderDetails (SalesOrderId, ProductId, Quantity, UnitPrice) VALUES
(1, 1, 2, 15000000),
(2, 7, 1, 30000000),
(2, 2, 1, 15000000),
(3, 4, 10, 1500000),
(4, 12, 1, 60000000),
(5, 23, 1, 12000000),
(6, 10, 1, 25000000),
(7, 8, 1, 35000000),
(8, 3, 1, 18000000),
(9, 13, 1, 5000000);
");
                ExecuteNonQuery(conn, "PRAGMA foreign_keys = ON;");
            }

            // Seed Promotions
            SeedPromotions(conn);
        }

        /// <summary>
        /// Seed 8 sample employees with properly hashed passwords using PBKDF2.
        /// Only seeds employees that don't already exist.
        /// </summary>
        private static void SeedEmployees(SQLiteConnection conn)
        {
            // Employee data: username, password, fullName, email, phone, roleId
            var employees = new[]
            {
                new { Username = "admin",      Password = "admin123",      FullName = "Nguyễn Văn Admin",     Email = "admin@schooldevice.vn",      Phone = "0901234567", RoleId = 1 },
                new { Username = "manager1",   Password = "manager123",    FullName = "Trần Thị Quản Lý",    Email = "manager@schooldevice.vn",    Phone = "0912345678", RoleId = 2 },
                new { Username = "sale1",      Password = "sale123",       FullName = "Lê Văn Bán Hàng",     Email = "sale1@schooldevice.vn",      Phone = "0923456789", RoleId = 3 },
                new { Username = "sale2",      Password = "sale123",       FullName = "Phạm Thị Kinh Doanh", Email = "sale2@schooldevice.vn",      Phone = "0934567890", RoleId = 3 },
                new { Username = "warehouse1", Password = "warehouse123",  FullName = "Hoàng Văn Kho",       Email = "warehouse@schooldevice.vn",  Phone = "0945678901", RoleId = 4 },
                new { Username = "kttoan1",    Password = "accountant123", FullName = "Ngô Thị Kế Toán",     Email = "accountant@schooldevice.vn", Phone = "0956789012", RoleId = 5 },
                new { Username = "sale3",      Password = "sale123",       FullName = "Vũ Minh Đức",         Email = "duc.vu@schooldevice.vn",     Phone = "0967890123", RoleId = 3 },
                new { Username = "manager2",   Password = "manager123",    FullName = "Đỗ Anh Tuấn",        Email = "tuan.do@schooldevice.vn",    Phone = "0978901234", RoleId = 2 },
            };

            foreach (var emp in employees)
            {
                var exists = GetCount(conn, "SELECT COUNT(1) FROM Employees WHERE Username = '" + emp.Username.Replace("'", "''") + "'");
                if (exists > 0) continue;

                // Generate proper PBKDF2 password hash
                byte[] salt = new byte[16];
                using (var rng = new RNGCryptoServiceProvider())
                {
                    rng.GetBytes(salt);
                }
                byte[] hash;
                using (var pbkdf2 = new Rfc2898DeriveBytes(emp.Password, salt, 10000))
                {
                    hash = pbkdf2.GetBytes(32);
                }

                using (var cmd = new SQLiteCommand(@"
INSERT INTO Employees (Username, PasswordHash, PasswordSalt, FullName, Email, Phone, RoleId, CreatedAt, IsActive)
VALUES (@username, @hash, @salt, @fullname, @email, @phone, @roleId, CURRENT_TIMESTAMP, 1);", conn))
                {
                    cmd.Parameters.AddWithValue("@username", emp.Username);
                    cmd.Parameters.AddWithValue("@hash", hash);
                    cmd.Parameters.AddWithValue("@salt", salt);
                    cmd.Parameters.AddWithValue("@fullname", emp.FullName);
                    cmd.Parameters.AddWithValue("@email", emp.Email);
                    cmd.Parameters.AddWithValue("@phone", emp.Phone);
                    cmd.Parameters.AddWithValue("@roleId", emp.RoleId);
                    cmd.ExecuteNonQuery();
                }

                Console.WriteLine("[DEBUG] Seeded employee: " + emp.Username + " (Role: " + emp.RoleId + ")");
            }
        }

        /// <summary>
        /// Seed 5 sample promotions.
        /// </summary>
        private static void SeedPromotions(SQLiteConnection conn)
        {
            if (GetCount(conn, "SELECT COUNT(1) FROM Promotions") > 0) return;

            ExecuteNonQuery(conn, @"
INSERT INTO Promotions (PromotionCode, PromotionName, Description, DiscountType, DiscountValue, MinOrderAmount, MaxDiscountAmount, StartDate, EndDate, UsageLimit, UsageCount, IsActive, AppliesTo, ApplyTargetId, CreatedBy) VALUES
('SUMMER2026', 'Khuyến mãi Hè 2026', 'Giảm 10% cho tất cả đơn hàng mùa hè', 'Percentage', 10, 5000000, 5000000, date('now', '-10 days'), date('now', '+80 days'), 100, 5, 1, 'All', NULL, 1),
('BIGORDER', 'Ưu đãi đơn lớn', 'Giảm trực tiếp 2 triệu cho đơn hàng trên 50 triệu', 'FixedAmount', 2000000, 50000000, NULL, date('now', '-5 days'), date('now', '+60 days'), 50, 2, 1, 'All', NULL, 1),
('STEM50', 'Ưu đãi thiết bị STEM', 'Giảm 15% cho thiết bị STEM', 'Percentage', 15, 1000000, 3000000, date('now'), date('now', '+45 days'), NULL, 0, 1, 'Category', 5, 1),
('WELCOME', 'Chào mừng khách mới', 'Giảm 5% cho đơn hàng đầu tiên', 'Percentage', 5, 0, 2000000, date('now', '-30 days'), date('now', '+180 days'), 200, 12, 1, 'All', NULL, 1),
('EXPIRED01', 'Khuyến mãi Tết đã hết hạn', 'Chương trình giảm giá Tết 2026', 'Percentage', 20, 10000000, 10000000, date('now', '-120 days'), date('now', '-30 days'), 100, 45, 0, 'All', NULL, 1);
");
            Console.WriteLine("[DEBUG] Seeded 5 sample promotions");
        }

        private static void ExecuteNonQuery(SQLiteConnection conn, string sql)
        {
            using (var cmd = new SQLiteCommand(sql, conn))
            {
                cmd.ExecuteNonQuery();
            }
        }

        private static long GetCount(SQLiteConnection conn, string sql)
        {
            using (var cmd = new SQLiteCommand(sql, conn))
            {
                return (long)cmd.ExecuteScalar();
            }
        }

        private static void AddColumnIfMissing(SQLiteConnection conn, string tableName, string columnName, string columnDefinition)
        {
            try
            {
                using (var cmd = new SQLiteCommand($"PRAGMA table_info({tableName})", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var name = reader["name"]?.ToString();
                        if (string.Equals(name, columnName, StringComparison.OrdinalIgnoreCase))
                        {
                            return; // Column already exists!
                        }
                    }
                }

                // If column doesn't exist, add it
                Console.WriteLine($"[DEBUG] Migrating database: Adding column '{columnName}' to table '{tableName}'");
                using (var alterCmd = new SQLiteCommand($"ALTER TABLE {tableName} ADD COLUMN {columnName} {columnDefinition}", conn))
                {
                    alterCmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[WARNING] Migration check failed for table {tableName}, column {columnName}: {ex.Message}");
            }
        }
    }
}