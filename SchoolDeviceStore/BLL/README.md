BLL README

Files:
- AuthService.cs : Authentication service used by GUI to login users and manage passwords.

Usage:
- Reference DAL project and DTO project from BLL.
- Call `new AuthService().EnsureAdminExists()` at application startup to create a default admin for demo.

