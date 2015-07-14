using dax.Core;
using dax.Db.Connect;
using dax.Db.SqlServer;
using dax.Properties;

namespace dax.Db
{
    public sealed class ProviderFactory : IProviderFactory
    {
        public static readonly ProviderFactory Instance = new ProviderFactory();

        private ProviderFactory()
        {
        }

        public IDbProvider Create()
        {
            return new SqlServerProvider(Settings.Default.ConnectionString);
        }

        public IConnection NewConnection(string serverName, string dbName, string login, string password)
        {
            throw new System.NotImplementedException();
        }
    }
}
