using System;
using System.Data;
using DAL;

namespace SchoolDeviceStore.Tests
{
    class DbQuery
    {
        static void Main()
        {
            Console.WriteLine("Testing database initialization and querying...\n");

            try
            {
                // First, ensure the database is created
                Console.WriteLine("Step 1: Initializing database schema...");
                DemoDatabaseInitializer.EnsureCreated();
                Console.WriteLine("✓ Schema initialization complete\n");

                // Check Roles
                var roles = DbHelper.ExecuteQuery("SELECT * FROM Roles");
                Console.WriteLine($"Roles table: {roles.Rows.Count} records");
                foreach (DataRow row in roles.Rows)
                    Console.WriteLine($"  - {row["RoleName"]}");

                // Check Employees
                var emps = DbHelper.ExecuteQuery("SELECT * FROM Employees");
                Console.WriteLine($"\nEmployees table: {emps.Rows.Count} records");
                foreach (DataRow row in emps.Rows)
                    Console.WriteLine($"  - {row["Username"]} ({row["FullName"]})");

                // Check Categories
                var cats = DbHelper.ExecuteQuery("SELECT * FROM Categories");
                Console.WriteLine($"\nCategories table: {cats.Rows.Count} records");
                foreach (DataRow row in cats.Rows)
                    Console.WriteLine($"  - {row["CategoryName"]}");

                // Check Suppliers
                var sups = DbHelper.ExecuteQuery("SELECT * FROM Suppliers");
                Console.WriteLine($"\nSuppliers table: {sups.Rows.Count} records");
                foreach (DataRow row in sups.Rows)
                    Console.WriteLine($"  - {row["SupplierName"]}");

                // Check Products
                var prods = DbHelper.ExecuteQuery("SELECT * FROM Products");
                Console.WriteLine($"\nProducts table: {prods.Rows.Count} records");
                foreach (DataRow row in prods.Rows)
                    Console.WriteLine($"  - {row["ProductName"]} ({row["ProductCode"]})");

                Console.WriteLine("\n✓ Database initialization successful!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Error: {ex.Message}");
                Console.WriteLine($"Stack: {ex.StackTrace}");
                Environment.Exit(1);
            }
        }
    }
}

