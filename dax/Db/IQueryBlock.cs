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
            set;
        }

        bool IsEmpty
        {
            get;
        }

        string QueryText
        {
            get;
        }

        /// <summary>
        /// Executed time in milliseconds
        /// </summary>
        long ElapsedTime
        {
            get;
        }

        void Update();

        void NextPage();

        void PrevPage();

        Task UpdateAsync();

        Task NextPageAsync();

        Task PrevPageAsync();
    }
}
