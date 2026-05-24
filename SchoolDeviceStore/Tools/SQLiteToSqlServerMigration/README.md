# SQLite -> SQL Server Migration Tool

This is a small console tool to migrate data from a SQLite database file to an existing SQL Server database using `SqlBulkCopy`.

Prerequisites
- .NET Framework 4.8 SDK / MSBuild to build the project
- `System.Data.SQLite.Core` NuGet package (declared in the project)
- A SQL Server instance and a target database with schema already created

Build

Use MSBuild or Visual Studio to build the project. Example (Developer Command Prompt):

```powershell
msbuild SQLiteToSqlServerMigration.csproj /p:Configuration=Release
```

Run

```powershell
SQLiteToSqlServerMigration.exe "C:\path\to\SchoolDeviceStore.db" "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=SchoolDeviceStore;Integrated Security=True;" 1000

# To create destination tables automatically (simple type mapping) before bulk copy:
SQLiteToSqlServerMigration.exe "C:\path\to\SchoolDeviceStore.db" "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=SchoolDeviceStore;Integrated Security=True;" 1000 true
```

Notes
- The tool currently assumes the target schema (tables and columns) already exist in SQL Server with matching column names.
- For large data sets you may want to increase `batchSize` and ensure appropriate indexes exist on destination tables.
- Next steps: implement schema conversion (CREATE TABLE translation), handle type mapping, and add per-table transforms.
