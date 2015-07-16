using System.Data;
using System.Threading.Tasks;

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

        bool IsEmpty
        {
            get;
        }

        string QueryText
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
