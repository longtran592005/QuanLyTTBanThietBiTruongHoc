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

Publish to one EXE
1. Open PowerShell in `SchoolDeviceStore/`.
2. Run `powershell -ExecutionPolicy Bypass -File .\Scripts\PublishSingleExe.ps1`.
3. The distribution EXE is generated at `publish\single-exe\SchoolDeviceStore.GUI.exe`.
 
Build setup installer (.exe)
1. Install Inno Setup 6 on the build machine.
2. Run `powershell -ExecutionPolicy Bypass -File .\Scripts\CreateInstaller.ps1`.
3. The installer is generated at `publish\installer\SchoolDeviceStoreSetup.exe`.
4. Installer behavior:
	- Creates Start Menu shortcut.
	- Optional Desktop shortcut.
	- Installs per-user under `%LOCALAPPDATA%\Programs\SchoolDeviceStore`.
	- Checks .NET Framework 4.8 before install.

Deploy to other computers
1. Install .NET Framework 4.8 Runtime on the target machine.
2. Copy only `SchoolDeviceStore.GUI.exe` to target machine and run it.
3. On first run, app auto-creates SQLite database and demo data under `%LOCALAPPDATA%\SchoolDeviceStore\Data`.
4. Logs are written to `%LOCALAPPDATA%\SchoolDeviceStore\Logs`.

Stability checklist for target machines
- Grant user write permission to `%LOCALAPPDATA%` (default Windows user profile already supports this).
- Add antivirus exclusion for `%LOCALAPPDATA%\SchoolDeviceStore\` if endpoint policy blocks local SQLite file write.

