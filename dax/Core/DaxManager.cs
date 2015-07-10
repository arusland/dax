using dax.Core.Document;
using dax.Db;
using System;
using System.Linq;
using System.Collections.Generic;

namespace dax.Core
{
    public class DaxManager
    {
        private const String PROPERTY_QUERY_VERSION = "version.scanner";
        private readonly IProviderFactory _dbProviderFactory;
        private readonly DaxDocument _document;

        public DaxManager(IProviderFactory dbProviderFactory, String filePath)
        {
            _dbProviderFactory = dbProviderFactory;
            _document = DaxDocument.Load(filePath);
        }        

        public IEnumerable<Input> Inputs
        {
            get { return _document.Inputs; }
        }

        public String FilePath
        {
            get
            {
                return _document.FilePath;
            }
        }

        public void Reload(Dictionary<String, String> inputValues)
        {
            IDbProvider dbProvider = _dbProviderFactory.Create();
            String version = GetVersion(dbProvider);
            Scope scope = GetScope(version);
            
            // TODO: !!!
        }

        private Scope GetScope(String version)
        {
            if (String.IsNullOrEmpty(version))
            {
                Scope scope = _document.Scopes.FirstOrDefault(p => p.Version == version);

                if (scope == null)
                {
                    throw new InvalidOperationException(String.Format("Scope for version '{0}' not found", version));
                }

                return scope;
            }
            else
            {
                Scope scope = _document.Scopes.FirstOrDefault(p => String.IsNullOrEmpty(p.Version));

                if (scope == null)
                {
                    throw new InvalidOperationException("Scope not found.");
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
    }
}
