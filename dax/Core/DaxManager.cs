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
        private readonly IProviderFactory _dbProviderFactory;
        private DaxDocument _document;
        private readonly TaskScheduler _uiContext;
        private readonly String _filePath;
        public event EventHandler<QueryReloadedEventArgs> OnQueryReloaded;
        public event EventHandler<NewBlockAddedEventArgs> OnNewBlockAdded;
        public event EventHandler<ErrorEventArgs> OnError;

        public DaxManager(IProviderFactory dbProviderFactory, String filePath, TaskScheduler uiContext)
        {
            _filePath = filePath;
            _dbProviderFactory = dbProviderFactory;
            _uiContext = uiContext;
        }        

        public IEnumerable<Input> Inputs
        {
            get 
            {
                CheckLoadedDocument();
                return _document.Inputs; 
            }
        }

        public String FilePath
        {
            get
            {
                return _filePath;
            }
        }

        public String DocumentName
        {
            get
            {
                CheckLoadedDocument();
                return _document.Name;
            }
        }

        public void Reload(Dictionary<String, String> inputValues)
        {
            CheckLoadedDocument();

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

            IDbProvider dbProvider = _dbProviderFactory.Create();
            String version = GetVersion(dbProvider);
            Scope scope = GetScope(version);
            Dictionary<Block, IQueryBlock> acceptedBlocks = new Dictionary<Block, IQueryBlock>();

            foreach(Block block in scope.Blocks)
            {
                if (block.CanAccept(inputValues))
                {
                    String query = block.Query.BuildQuery(inputValues);
                    acceptedBlocks.Add(block, dbProvider.CreateBlock(query));
                }
            }
            
            foreach (var item in acceptedBlocks)
            {
                item.Value.Update();
                DoNewBlockAddedEvent(item.Key, item.Value);           
            }
        }

        public async Task ReloadAsync(Dictionary<String, String> inputValues)
        {
            Task task = Task.Factory.StartNew(() => Reload(inputValues));

            await task;
        }

        private void CheckLoadedDocument()
        {
            if (_document == null)
            {
                try
                {
                    _document = DaxDocument.Load(_filePath);                    
                }
                catch (Exception ex)
                {
                    DoErrorEvent(ex.Message);
                }
            }
        }

        private void DoQueryReloadedEvent()
        {
            RunOnUIContext(() => OnQueryReloaded(this, new QueryReloadedEventArgs()));
        }

        private void DoNewBlockAddedEvent(Block block, IQueryBlock queryBlock)
        {
            RunOnUIContext(() => OnNewBlockAdded(this, new NewBlockAddedEventArgs(block, queryBlock)));
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

        private void RunOnUIContext(Action action)
        {
            var token = Task.Factory.CancellationToken;
            Task.Factory.StartNew(() =>
            {
                action();
            }, token, TaskCreationOptions.None, _uiContext);
        }
    }
}
