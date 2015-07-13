using System;

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
    }
}
