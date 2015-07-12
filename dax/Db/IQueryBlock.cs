using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace dax.Db
{
    public interface IQueryBlock
    {
        DataTable Table
        {
            get;
        }

        int PageSize
        {
            get;
        }

        int PageIndex
        {
            get;
        }

        void Update();

        void NextPage();

        void PrevPage();

        Task NextPageAsync();

        Task PrevPageAsync();
    }
}
