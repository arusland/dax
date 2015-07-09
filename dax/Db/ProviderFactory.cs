using dax.Db.SqlServer;

namespace dax.Db
{
    public sealed class ProviderFactory
    {
        public static readonly ProviderFactory Instance = new ProviderFactory();

        private ProviderFactory()
        {
        }

        public IDbProvider Create()
        {
            return new SqlServerProvider("Data Source=(local);Initial Catalog=TestDb;Integrated Security=true");
        }
    }
}
