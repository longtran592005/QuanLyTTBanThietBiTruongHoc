namespace BLL
{
    public class DatabaseSetupService
    {
        public void EnsureDemoDatabaseReady()
        {
            var provider = System.Configuration.ConfigurationManager.ConnectionStrings["SchoolDeviceStoreDB"]?.ProviderName ?? string.Empty;
            if (provider.IndexOf("SqlClient", System.StringComparison.OrdinalIgnoreCase) >= 0)
            {
                // SQL Server migration already created schema and data.
                // Keep this as a no-op to avoid reseeding SQLite-specific demo data.
                return;
            }

            DAL.DemoDatabaseInitializer.EnsureCreated();
        }
    }
}