Run Guide — School Device Store

Prerequisites
- Visual Studio 2026 installed
- .NET Framework 4.8 developer pack installed

Open the solution
1. Open Visual Studio 2026.
2. Choose File > Open > Project/Solution.
3. Select `d:\LTNC\SchoolDeviceStore\SchoolDeviceStore.sln`.
4. Allow Visual Studio to restore and load the projects.
5. If needed, set `SchoolDeviceStore.GUI` as the startup project.

Steps to prepare database (SQLite demo)
1. Build the solution once so the SQLite provider DLLs are restored.
2. Run `Scripts/CreateSQLiteDemoDb.ps1` from the solution root.
3. Confirm the database file exists at `GUI.WinForms/bin/Debug/net48/SchoolDeviceStore.db`.
4. The GUI connection string already points to `|DataDirectory|\SchoolDeviceStore.db`.

Login for demo
- Username: `admin`
- Password: `admin123`

Implemented screens
- Login
- Dashboard
- Product management
- Category management
- Supplier management
- Sales / invoice screen
- Backup / restore screen
- Reports screen

Backup and restore notes
- Backup copies the SQLite `.db` file to the path you choose.
- Restore replaces the active SQLite `.db` file with the backup file.
- For restore, close the app before replacing the database file.

Build and run
1. Press `F5` to build and run with debugging.
2. Press `Ctrl+F5` to run without debugging.

Notes
- The current implementation uses SQLite for the demo route and targets .NET Framework 4.8.

