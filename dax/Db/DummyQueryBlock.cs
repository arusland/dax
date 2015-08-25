using System;
using System.Threading.Tasks;

namespace dax.Db
{
    public sealed class DummyQueryBlock : IQueryBlock
    {
        public readonly static DummyQueryBlock Instance = new DummyQueryBlock();

        private DummyQueryBlock()
        {
        }

        public System.Data.DataTable Table
        {
            get { return null; }
        }

        public int PageSize
        {
            get { return 0; }
        }

        public int PageIndex
        {
            get
            {
                return 0;
            }
            set
            {
            }
        }

        public bool IsEmpty
        {
            get { return true; }
        }

        public string QueryText
        {
            get { return "[Query skipped]"; }
        }

        public long ElapsedTime
        {
            get { return 0; }
        }

        public void Update()
        {
        }

        public void NextPage()
        {
        }

        public void PrevPage()
        {
        }

        public Task UpdateAsync()
        {
            return CreateDummyTask();
        }

        public Task NextPageAsync()
        {
            return CreateDummyTask();
        }

        public Task PrevPageAsync()
        {
            return CreateDummyTask();
        }

        private static Task CreateDummyTask()
        {
            return Task.Factory.StartNew(() => { });
        }
    }
}
