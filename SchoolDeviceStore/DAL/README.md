DAL README

Files:
- DbHelper.cs : A simple ADO.NET helper (ExecuteQuery/ExecuteNonQuery/ExecuteScalar)
- Repositories/EmployeeRepository.cs : CRUD operations for Employees (parameterized queries)
- Utils/PasswordHelper.cs : PBKDF2-based password hash/verify helper

Usage:
1. Copy these files into a Class Library project targeting .NET Framework 4.8.
2. Ensure you reference `System.Configuration` and add the connection string named "SchoolDeviceStore" to your App.config (see Config/App.config template).
3. Build the DAL project and reference it from BLL and GUI projects.

Security notes:
- Passwords must be hashed using `PasswordHelper.CreatePasswordHash` before saving.
- Always use parameterized queries (as shown) to avoid SQL injection.

