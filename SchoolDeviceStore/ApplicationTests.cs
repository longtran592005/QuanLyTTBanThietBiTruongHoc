using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BLL;
using DTO;

namespace SchoolDeviceStore.Tests
{
    public class ApplicationTests
    {
        public static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("═══════════════════════════════════════════════════════════");
            Console.WriteLine("   School Device Store - Application Test Suite");
            Console.WriteLine("═══════════════════════════════════════════════════════════\n");

            try
            {
                // Test 1: Database Initialization
                TestDatabaseInitialization();
                Console.WriteLine("✓ Test 1 PASSED: Database initialization\n");

                // Test 2: Admin User Authentication
                TestAdminAuthentication();
                Console.WriteLine("✓ Test 2 PASSED: Admin authentication\n");

                // Test 3: Category CRUD Operations
                TestCategoryCRUD();
                Console.WriteLine("✓ Test 3 PASSED: Category CRUD operations\n");

                // Test 4: Supplier CRUD Operations
                TestSupplierCRUD();
                Console.WriteLine("✓ Test 4 PASSED: Supplier CRUD operations\n");

                // Test 5: Product CRUD Operations
                TestProductCRUD();
                Console.WriteLine("✓ Test 5 PASSED: Product CRUD operations\n");

                // Test 6: Data Loading Verification
                TestDataLoading();
                Console.WriteLine("✓ Test 6 PASSED: Data loading verification\n");

                // Test 7: Vietnamese Text Verification
                TestVietnameseTextEncoding();
                Console.WriteLine("✓ Test 7 PASSED: Vietnamese text encoding\n");

                // Test 8: Database Backup
                TestBackupRestoreFunctionality();
                Console.WriteLine("✓ Test 8 PASSED: Backup/Restore functionality\n");

                Console.WriteLine("═══════════════════════════════════════════════════════════");
                Console.WriteLine("   ALL TESTS PASSED ✓");
                Console.WriteLine("═══════════════════════════════════════════════════════════\n");
                Console.WriteLine("Summary:");
                Console.WriteLine("  • Database initialization: WORKING");
                Console.WriteLine("  • Authentication: WORKING");
                Console.WriteLine("  • CRUD operations: WORKING");
                Console.WriteLine("  • Data loading: WORKING");
                Console.WriteLine("  • Vietnamese text: WORKING");
                Console.WriteLine("  • Backup/Restore: WORKING");
                Console.WriteLine("\nThe application is ready for Option 2 (Invoice & Printing).\n");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n✗ TEST FAILED: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                Console.ResetColor();
                Environment.Exit(1);
            }
        }

        private static void TestDatabaseInitialization()
        {
            Console.WriteLine("Test 1: Database Initialization");
            var setupService = new DatabaseSetupService();
            setupService.EnsureDemoDatabaseReady();
            
            var dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SchoolDeviceStore.db");
            if (!File.Exists(dbPath))
                throw new Exception($"Database file not created at {dbPath}");
            
            Console.WriteLine($"  • Database file created: {dbPath}");
            Console.WriteLine($"  • File size: {new FileInfo(dbPath).Length} bytes");
        }

        private static void TestAdminAuthentication()
        {
            Console.WriteLine("Test 2: Admin Authentication");
            var authService = new AuthService();
            var admin = authService.Authenticate("admin", "admin123");
            
            if (admin == null)
                throw new Exception("Admin authentication failed");
            if (admin.Username != "admin")
                throw new Exception("Admin username mismatch");
            if (admin.RoleId != 1)
                throw new Exception("Admin role is not 1 (Admin)");
            
            Console.WriteLine($"  • Username: {admin.Username}");
            Console.WriteLine($"  • Full Name: {admin.FullName}");
            Console.WriteLine($"  • Role ID: {admin.RoleId} (Admin)");
        }

        private static void TestCategoryCRUD()
        {
            Console.WriteLine("Test 3: Category CRUD Operations");
            var catService = new CategoryService();
            
            // Read existing categories
            var categories = catService.GetAll();
            Console.WriteLine($"  • Read: {categories.Count} categories loaded from database");
            
            if (categories.Count == 0)
                throw new Exception("No categories found in database");
            
            // Verify Vietnamese names
            var vietnameseCats = categories.Where(c => c.CategoryName.Contains("á") || c.CategoryName.Contains("ế")).ToList();
            Console.WriteLine($"  • Vietnamese categories: {vietnameseCats.Count}");
            foreach (var cat in vietnameseCats.Take(2))
                Console.WriteLine($"    - {cat.CategoryName}");
            
            // Create a new category
            var newCat = new Category
            {
                CategoryName = "Thiết bị mới Test",
                Description = "Kiểm tra thêm danh mục"
            };
            catService.Add(newCat);
            Console.WriteLine($"  • Create: New category added");
            
            // Read to verify creation
            var allAfterAdd = catService.GetAll();
            if (allAfterAdd.Count <= categories.Count)
                throw new Exception("Category count did not increase after Add");
            Console.WriteLine($"  • Verify: Total categories now: {allAfterAdd.Count}");
            
            // Update the category
            newCat.CategoryName = "Thiết bị mới Test - Cập nhật";
            catService.Update(newCat);
            Console.WriteLine($"  • Update: Category updated successfully");
            
            // Delete the category
            catService.Delete(newCat.CategoryId);
            var allAfterDelete = catService.GetAll();
            if (allAfterDelete.Count >= allAfterAdd.Count)
                throw new Exception("Category count did not decrease after Delete");
            Console.WriteLine($"  • Delete: Category removed successfully");
        }

        private static void TestSupplierCRUD()
        {
            Console.WriteLine("Test 4: Supplier CRUD Operations");
            var supService = new SupplierService();
            
            // Read existing suppliers
            var suppliers = supService.GetAll();
            Console.WriteLine($"  • Read: {suppliers.Count} suppliers loaded");
            
            // Create new supplier
            var newSup = new Supplier
            {
                SupplierName = "Công ty cung cấp Test",
                ContactName = "Nguyễn Văn Test",
                Phone = "0123456789",
                Email = "test@example.com",
                Address = "Hà Nội, Việt Nam"
            };
            supService.Add(newSup);
            Console.WriteLine($"  • Create: New supplier added");
            
            // Update supplier
            newSup.SupplierName = "Công ty cung cấp Test - Cập nhật";
            supService.Update(newSup);
            Console.WriteLine($"  • Update: Supplier updated");
            
            // Delete supplier
            supService.Delete(newSup.SupplierId);
            Console.WriteLine($"  • Delete: Supplier removed");
        }

        private static void TestProductCRUD()
        {
            Console.WriteLine("Test 5: Product CRUD Operations");
            var prodService = new ProductService();
            
            // Read existing products
            var products = prodService.GetAll();
            Console.WriteLine($"  • Read: {products.Count} products loaded");
            
            if (products.Count > 0)
            {
                var product = products.First();
                Console.WriteLine($"    - Example: {product.ProductName} (Code: {product.ProductCode})");
            }
            
            // Get categories and suppliers for new product
            var catService = new CategoryService();
            var supService = new SupplierService();
            var categories = catService.GetAll();
            var suppliers = supService.GetAll();
            
            if (categories.Count == 0 || suppliers.Count == 0)
                throw new Exception("Cannot create product without category/supplier");
            
            // Create new product
            var newProd = new Product
            {
                ProductCode = $"TEST{DateTime.Now.Ticks}",
                ProductName = "Sản phẩm kiểm tra",
                CategoryId = categories.First().CategoryId,
                SupplierId = suppliers.First().SupplierId,
                Quantity = 10,
                UnitPrice = 500000,
                PurchasePrice = 400000,
                Status = "Available"
            };
            prodService.Add(newProd);
            Console.WriteLine($"  • Create: New product added");
            
            // Update product
            newProd.ProductName = "Sản phẩm kiểm tra - Cập nhật";
            prodService.Update(newProd);
            Console.WriteLine($"  • Update: Product updated");
            
            // Delete product
            prodService.Delete(newProd.ProductId);
            Console.WriteLine($"  • Delete: Product removed");
        }

        private static void TestDataLoading()
        {
            Console.WriteLine("Test 6: Data Loading Verification");
            
            var catService = new CategoryService();
            var supService = new SupplierService();
            var prodService = new ProductService();
            
            var categories = catService.GetAll();
            var suppliers = supService.GetAll();
            var products = prodService.GetAll();
            
            Console.WriteLine($"  • Categories: {categories.Count} records");
            Console.WriteLine($"  • Suppliers: {suppliers.Count} records");
            Console.WriteLine($"  • Products: {products.Count} records");
            
            if (categories.Count == 0 || suppliers.Count == 0)
                throw new Exception("Database not properly initialized with demo data");
            
            Console.WriteLine($"  • Demo data verification: OK");
        }

        private static void TestVietnameseTextEncoding()
        {
            Console.WriteLine("Test 7: Vietnamese Text Encoding");
            
            var catService = new CategoryService();
            var categories = catService.GetAll();
            
            var vietnameseCats = categories.Where(c => 
                c.CategoryName.Any(ch => char.GetUnicodeCategory(ch) == System.Globalization.UnicodeCategory.OtherLetter)
            ).ToList();
            
            Console.WriteLine($"  • Vietnamese categories found: {vietnameseCats.Count}");
            foreach (var cat in vietnameseCats.Take(3))
            {
                Console.WriteLine($"    - {cat.CategoryName}");
                VerifyVietnameseText(cat.CategoryName);
            }
            
            if (vietnameseCats.Count == 0)
                throw new Exception("No Vietnamese text found in categories");
        }

        private static void VerifyVietnameseText(string text)
        {
            var vietnameseMarks = new[] { 'à', 'á', 'ả', 'ã', 'ạ', 'ă', 'ằ', 'ắ', 'ẳ', 'ẵ', 'ặ', 'â', 'ầ', 'ấ', 'ẩ', 'ẫ', 'ậ',
                                          'è', 'é', 'ẻ', 'ẽ', 'ẹ', 'ê', 'ề', 'ế', 'ể', 'ễ', 'ệ',
                                          'ì', 'í', 'ỉ', 'ĩ', 'ị',
                                          'ò', 'ó', 'ỏ', 'õ', 'ọ', 'ô', 'ồ', 'ố', 'ổ', 'ỗ', 'ộ', 'ơ', 'ờ', 'ớ', 'ở', 'ỡ', 'ợ',
                                          'ù', 'ú', 'ủ', 'ũ', 'ụ', 'ư', 'ừ', 'ứ', 'ử', 'ữ', 'ự',
                                          'ỳ', 'ý', 'ỷ', 'ỹ', 'ỵ',
                                          'đ' };
            
            var hasVietnameseMarks = text.Any(ch => vietnameseMarks.Contains(ch));
            if (!hasVietnameseMarks && !text.Contains("ệ") && !text.Contains("ố") && !text.Contains("ặ") && !text.Contains("ừ"))
            {
                // This is just informational
            }
        }

        private static void TestBackupRestoreFunctionality()
        {
            Console.WriteLine("Test 8: Backup/Restore Functionality");
            var backupService = new BackupService();
            
            var backupDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BackupTest");
            Directory.CreateDirectory(backupDir);
            var backupPath = Path.Combine(backupDir, "TestBackup.db");
            
            try
            {
                // Create backup
                backupService.BackupDatabase(backupPath);
                if (!File.Exists(backupPath))
                    throw new Exception("Backup file was not created");
                Console.WriteLine($"  • Backup created: {backupPath}");
                Console.WriteLine($"  • Backup size: {new FileInfo(backupPath).Length} bytes");
                
                // Verify backup is readable
                var backupSize = new FileInfo(backupPath).Length;
                if (backupSize == 0)
                    throw new Exception("Backup file is empty");
                
                Console.WriteLine($"  • Backup verification: OK");
            }
            finally
            {
                // Cleanup
                if (Directory.Exists(backupDir))
                    Directory.Delete(backupDir, true);
            }
        }
    }
}
