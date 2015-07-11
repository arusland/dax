using dax.Db;

namespace dax.Db
{
    public interface IProviderFactory
    {
        IDbProvider Create();
    }
}
