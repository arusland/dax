using System;
using System.Threading.Tasks;

namespace dax.Db
{
    public sealed class DummyQueryBlock : IQueryBlock
    {
        public readonly static DummyQueryBlock SkippedInstance = new DummyQueryBlock(true, "[Query skipped]");

        private readonly String _query;
        private readonly bool _skipped;

        private DummyQueryBlock(bool skipped, String query)
        {
            _skipped = skipped;
            _query = query;
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

        public bool IsSkipped
        {
            get { return _skipped; }
        }

        public string QueryText
        {
            get { return _query; }
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

        public static DummyQueryBlock Make(String query)
        {
            return new DummyQueryBlock(false, query);
        }
    }
}
