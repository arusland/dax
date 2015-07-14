using System;
using System.Threading.Tasks;

namespace dax.Db.Connect
{
    public interface IConnection
    {
        String ServerName
        {
            get;
        }

        String DbName
        {
            get;
        }

        String Login
        {
            get;
        }

        String Password
        {
            get;
        }


        String ConnectionString
        {
            get;
        }

        Task<String> Test();
    }
}
