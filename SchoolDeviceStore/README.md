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
- Single EXE publish script: `Scripts/PublishSingleExe.ps1`
- Run guide: `Docs/RunGuide.md`
- Project report: `Docs/ProjectReport.md`

Single EXE publish (for distribution)
1. Open PowerShell at solution root.
2. Run: `powershell -ExecutionPolicy Bypass -File .\Scripts\PublishSingleExe.ps1`
3. Output file: `publish\single-exe\SchoolDeviceStore.GUI.exe`

Cross-machine runtime notes
- This executable targets .NET Framework 4.8.
- On target machine, install .NET Framework 4.8 Runtime if missing.
- App stores data and logs per-user at `%LOCALAPPDATA%\SchoolDeviceStore\`:
	- Database: `%LOCALAPPDATA%\SchoolDeviceStore\Data\SchoolDeviceStore.db`
	- Logs: `%LOCALAPPDATA%\SchoolDeviceStore\Logs\application.log`

Installer EXE (recommended for end users)
1. Install Inno Setup 6 on your build machine.
2. Run: `powershell -ExecutionPolicy Bypass -File .\Scripts\CreateInstaller.ps1`
3. Output installer: `publish\installer\SchoolDeviceStoreSetup.exe`
4. End users only need to run setup, click Next/Install, then launch from Desktop or Start Menu shortcut.

Next steps
1. Add reporting/printing/export modules.
2. Polish the dashboard, icons, and visual theme.
3. Expand the demo database schema and seed data.

