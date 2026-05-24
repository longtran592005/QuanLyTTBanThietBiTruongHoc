using System;
using System.Data;
using System.Data.SQLite;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace SQLiteToSqlServerMigration
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: SQLiteToSqlServerMigration <source-sqlite-file> <destination-sqlserver-connection-string> [batchSize] [createTables:true|false]");
                return 1;
            }

            var sqliteFile = args[0];
            var destConnString = args[1];
            var batchSize = 1000;
            if (args.Length >= 3 && int.TryParse(args[2], out var parsed)) batchSize = parsed;
            var createTables = false;
            if (args.Length >= 4) bool.TryParse(args[3], out createTables);

            var sourceConnString = $"Data Source={sqliteFile};Version=3;";

            Console.WriteLine($"Source: {sqliteFile}");
            Console.WriteLine($"Destination: {destConnString}");
            Console.WriteLine($"Batch size: {batchSize}");

            try
            {
                using (var source = new SQLiteConnection(sourceConnString))
                using (var dest = new SqlConnection(destConnString))
                {
                    source.Open();
                    dest.Open();

                    // Ensure destination database exists (create if missing)
                    EnsureDatabaseExists(destConnString);

                    var tables = GetSqliteTableNames(source);
                    Console.WriteLine($"Found {tables.Count} tables to migrate.");

                    if (createTables)
                    {
                        Console.WriteLine("Creating tables on destination (if missing)...");
                        foreach (var t in tables)
                        {
                            var schema = GetSqliteTableSchema(source, t);
                            CreateSqlServerTableIfNotExists(dest, t, schema);
                        }
                    }

                    foreach (var table in tables)
                    {
                        Console.WriteLine($"Migrating table: {table}");
                        var dt = new DataTable();
                        using (var cmd = new SQLiteCommand($"SELECT * FROM [{table}];", source))
                        using (var adapter = new SQLiteDataAdapter(cmd))
                        {
                            adapter.Fill(dt);
                        }

                        Console.WriteLine($"  Rows read: {dt.Rows.Count}");
                        if (dt.Rows.Count == 0)
                        {
                            Console.WriteLine("  Skipping empty table.");
                            continue;
                        }

                        using (var bulk = new SqlBulkCopy(dest))
                        {
                            bulk.DestinationTableName = table;
                            bulk.BatchSize = batchSize;

                            // Add column mappings for matching column names
                            foreach (DataColumn col in dt.Columns)
                            {
                                bulk.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                            }

                            try
                            {
                                bulk.WriteToServer(dt);
                                Console.WriteLine($"  Wrote {dt.Rows.Count} rows to {table}.");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"  Error writing table {table}: {ex.Message}");
                            }
                        }
                    }
                }

                Console.WriteLine("Migration completed.");
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fatal error: {ex.Message}");
                return 2;
            }
        }

        static List<string> GetSqliteTableNames(SQLiteConnection conn)
        {
            var tables = new List<string>();
            using (var cmd = new SQLiteCommand("SELECT name FROM sqlite_master WHERE type='table' AND name NOT LIKE 'sqlite_%' ORDER BY name;", conn))
            using (var rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    tables.Add(rdr.GetString(0));
                }
            }
            return tables;
        }

        class ColumnDef
        {
            public string Name { get; set; }
            public string Type { get; set; }
            public bool NotNull { get; set; }
            public string DefaultValue { get; set; }
            public bool IsPrimaryKey { get; set; }
        }

        static List<ColumnDef> GetSqliteTableSchema(SQLiteConnection conn, string table)
        {
            var cols = new List<ColumnDef>();
            using (var cmd = new SQLiteCommand($"PRAGMA table_info([{table}]);", conn))
            using (var rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    cols.Add(new ColumnDef
                    {
                        Name = rdr[1].ToString(),
                        Type = rdr[2].ToString(),
                        NotNull = Convert.ToInt32(rdr[3]) == 1,
                        DefaultValue = rdr[4]?.ToString(),
                        IsPrimaryKey = Convert.ToInt32(rdr[5]) == 1
                    });
                }
            }
            return cols;
        }

        static void CreateSqlServerTableIfNotExists(SqlConnection dest, string table, List<ColumnDef> schema)
        {
            // Check existence
            var exists = false;
            using (var cmd = new SqlCommand("SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = @t", dest))
            {
                cmd.Parameters.AddWithValue("@t", table);
                exists = (int)cmd.ExecuteScalar() > 0;
            }

            if (exists)
            {
                Console.WriteLine($"  Table {table} already exists on destination.");
                return;
            }

            // Build CREATE TABLE statement
            var parts = new List<string>();
            foreach (var c in schema)
            {
                var sqlType = MapSqliteTypeToSqlServer(c.Type);
                // If primary key maps to NVARCHAR(MAX), change to NVARCHAR(450) for index/PK support
                if (c.IsPrimaryKey && string.Equals(sqlType, "NVARCHAR(MAX)", StringComparison.OrdinalIgnoreCase))
                {
                    sqlType = "NVARCHAR(450)";
                }
                var part = $"[{c.Name}] {sqlType}";
                if (c.IsPrimaryKey)
                {
                    if (sqlType.Equals("INT", StringComparison.OrdinalIgnoreCase))
                        part += " IDENTITY(1,1) PRIMARY KEY";
                    else
                        part += " PRIMARY KEY";
                }
                else
                {
                    part += c.NotNull ? " NOT NULL" : " NULL";
                }
                parts.Add(part);
            }

            var create = $"CREATE TABLE [{table}] ({string.Join(", ", parts)})";
            using (var cmd = new SqlCommand(create, dest))
            {
                cmd.ExecuteNonQuery();
            }
            Console.WriteLine($"  Created table {table} on destination.");
        }

        static string MapSqliteTypeToSqlServer(string sqliteType)
        {
            if (string.IsNullOrEmpty(sqliteType)) return "NVARCHAR(MAX)";
            var t = sqliteType.ToUpperInvariant();
            if (t.Contains("INT")) return "INT";
            if (t.Contains("CHAR") || t.Contains("CLOB") || t.Contains("TEXT")) return "NVARCHAR(MAX)";
            if (t.Contains("BLOB")) return "VARBINARY(MAX)";
            if (t.Contains("REAL") || t.Contains("FLOA") || t.Contains("DOUB")) return "FLOAT";
            if (t.Contains("NUMERIC") || t.Contains("DECIMAL")) return "DECIMAL(18,2)";
            return "NVARCHAR(MAX)";
        }

        static void EnsureDatabaseExists(string destConnString)
        {
            try
            {
                var builder = new SqlConnectionStringBuilder(destConnString);
                var database = builder.InitialCatalog;
                if (string.IsNullOrEmpty(database))
                {
                    Console.WriteLine("Destination connection string has no Initial Catalog; skipping DB create.");
                    return;
                }

                // Connect to master to create database if not exists
                var masterBuilder = new SqlConnectionStringBuilder(destConnString);
                masterBuilder.InitialCatalog = "master";
                using (var conn = new SqlConnection(masterBuilder.ConnectionString))
                using (var cmd = conn.CreateCommand())
                {
                    conn.Open();
                    cmd.CommandText = "SELECT COUNT(*) FROM sys.databases WHERE name = @name";
                    cmd.Parameters.AddWithValue("@name", database);
                    var exists = (int)cmd.ExecuteScalar() > 0;
                    if (!exists)
                    {
                        Console.WriteLine($"Database '{database}' does not exist. Creating...");
                        cmd.Parameters.Clear();
                        cmd.CommandText = $"CREATE DATABASE [{database}]";
                        cmd.ExecuteNonQuery();
                        Console.WriteLine($"Database '{database}' created.");
                    }
                    else
                    {
                        Console.WriteLine($"Database '{database}' already exists.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: could not ensure database exists: {ex.Message}");
            }
        }
    }
}
