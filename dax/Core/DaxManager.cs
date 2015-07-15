using dax.Core.Events;
using dax.Db;
using dax.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dax.Core
{
    public class DaxManager
    {
        private const String PROPERTY_QUERY_VERSION = "version.query";
        private DaxDocument _document;
        private readonly TaskScheduler _uiContext;
        public event EventHandler<QueryReloadedEventArgs> OnQueryReloaded;
        public event EventHandler<NewBlockAddedEventArgs> OnNewBlockAdded;
        public event EventHandler<ErrorEventArgs> OnError;
        public event EventHandler<QueryProviderEventArgs> OnQueryProvider;
        private IDbProvider _lastProvider;

        public DaxManager(String filePath, TaskScheduler uiContext)
        {
            _uiContext = uiContext;
            _document = DaxDocument.Load(filePath);
        }

        public IEnumerable<Input> Inputs
        {
            get
            {
                return _document.Inputs;
            }
        }

        public String FilePath
        {
            get
            {
                return _document.FilePath;
            }
        }

        public String DocumentName
        {
            get
            {
                return _document.Name;
            }
        }

        public bool IsConnected
        {
            get
            {
                return _lastProvider != null;
            }
        }

        public void Reload(Dictionary<String, String> inputValues)
        {
            try
            {
                ReloadInternal(inputValues);
            }
            catch (System.Exception ex)
            {
                DoErrorEvent(ex.Message);
            }
        }

        public void ReloadInternal(Dictionary<String, String> inputValues)
        {
            DoQueryReloadedEvent();

            IDbProvider dbProvider = GetQueryProvider();

            if (dbProvider != null)
            {
                String version = GetVersion(dbProvider);
                Scope scope = GetScope(version);
                Dictionary<Block, IQueryBlock> acceptedBlocks = new Dictionary<Block, IQueryBlock>();

                foreach (Block block in scope.Blocks)
                {
                    if (block.CanAccept(inputValues))
                    {
                        String query = block.Query.BuildQuery(inputValues);
                        acceptedBlocks.Add(block, dbProvider.CreateBlock(query));
                    }
                }

                foreach (var item in acceptedBlocks)
                {
                    Block block = item.Key;
                    IQueryBlock queryBlock = item.Value;

                    queryBlock.Update();

                    if (!queryBlock.IsEmpty || block.ShowOnEmpty)
                    {
                        bool canceled = DoNewBlockAddedEvent(block, queryBlock);

                        if (canceled)
                        {
                            // user canceled operation
                            break;
                        }
                    }
                }
            }
        }

        public async Task ReloadAsync(Dictionary<String, String> inputValues)
        {
            Task task = Task.Factory.StartNew(() => Reload(inputValues));

            await task;
        }

        public Task Reconnect()
        {
            return Task.Factory.StartNew(() =>
            {
                GetQueryProvider(true);
            });
        }

        private void DoQueryReloadedEvent()
        {
            RunOnUIContext(() => OnQueryReloaded(this, new QueryReloadedEventArgs()));
        }

        private IDbProvider GetQueryProvider(bool reset = false)
        {
            var ea = new QueryProviderEventArgs(this, reset);
            RunOnUIContext(() => OnQueryProvider(this, ea));

            if (ea.Provider != null)
            {
                _lastProvider = ea.Provider;
            }

            return ea.Provider;
        }

        private bool DoNewBlockAddedEvent(Block block, IQueryBlock queryBlock)
        {
            var ea = new NewBlockAddedEventArgs(block, queryBlock);
            RunOnUIContext(() => OnNewBlockAdded(this, ea));
            return ea.Canceled;
        }

        private void DoErrorEvent(String message)
        {
            RunOnUIContext(() => OnError(this, new ErrorEventArgs(message)));
        }

        private Scope GetScope(String version)
        {
            if (String.IsNullOrEmpty(version))
            {
                Scope scope = _document.Scopes.FirstOrDefault(p => String.IsNullOrEmpty(p.Version));

                if (scope == null)
                {
                    throw new InvalidOperationException("Scope not found.");
                }

                return scope;
            }
            else
            {
                Scope scope = _document.Scopes.FirstOrDefault(p => p.Version == version);

                if (scope == null)
                {
                    throw new InvalidOperationException(String.Format("Scope for version '{0}' not found", version));
                }

                return scope;
            }
        }

        private String GetVersion(IDbProvider dbProvider)
        {
            Property versionQuery = GetVersionProperty();
            String version = versionQuery != null ? (String)dbProvider.ExecuteScalar(versionQuery.Value) : null;

            return version;
        }

        private Property GetVersionProperty()
        {
            var prop = _document.Properties.FirstOrDefault(p => p.Name == PROPERTY_QUERY_VERSION);

            if (prop == null || String.IsNullOrWhiteSpace(prop.Value))
            {
                return null;
            }

            return prop;
        }

        private Task RunOnUIContext(Action action, bool isAsync = false)
        {
            var token = Task.Factory.CancellationToken;
            var task = Task.Factory.StartNew(() =>
            {
                action();
            }, token, TaskCreationOptions.None, _uiContext);

            if (!isAsync)
            {
                task.Wait();
            }

            return task;
        }
    }
}
