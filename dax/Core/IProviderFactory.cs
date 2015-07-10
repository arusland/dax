using dax.Db;

namespace dax.Core
{
    public interface IProviderFactory
    {
        IDbProvider Create();
    }
}
