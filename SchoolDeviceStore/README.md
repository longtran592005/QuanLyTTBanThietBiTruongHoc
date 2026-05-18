School Device Store — Desktop Management System

Overview
- WinForms 3-layer application (GUI / BLL / DAL)
- Target: Visual Studio 2026, .NET Framework 4.8
- Database: SQLite demo database for the current implementation path

Implemented modules
- Login and default admin bootstrap
- Product management
- Category and supplier management
- Sales/invoice creation
- SQLite file backup and restore

Demo login
- Username: `admin`
- Password: `admin123`

Key paths
- Solution root: `SchoolDeviceStore/`
- Database scripts: `Database/SQL/010_schema.sql`, `Database/SQL/020_sample_data.sql`
- SQLite seed script: `Scripts/CreateSQLiteDemoDb.ps1`
- Run guide: `Docs/RunGuide.md`
- Project report: `Docs/ProjectReport.md`

Next steps
1. Add reporting/printing/export modules.
2. Polish the dashboard, icons, and visual theme.
3. Expand the demo database schema and seed data.

