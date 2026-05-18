namespace BLL
{
    public class DatabaseSetupService
    {
        public void EnsureDemoDatabaseReady()
        {
            DAL.DemoDatabaseInitializer.EnsureCreated();
        }
    }
}