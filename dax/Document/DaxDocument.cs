using dax.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace dax.Document
{
    public class DaxDocument
    {
        private readonly List<Property> _properties = new List<Property>();        
        private readonly List<Input> _inputs = new List<Input>();        
        private readonly List<Scope> _scopes = new List<Scope>();
        private readonly FileInfo _currentFile;
        
        
        private DaxDocument(String file)
        {
            var fileInfo = new FileInfo(file);

            if (!fileInfo.Exists)
            {
                throw new FileNotFoundException(fileInfo.FullName);
            }

            Load(fileInfo);
            _currentFile = fileInfo;
        }

        public String Name
        {
            get;
            private set;
        }

        public String FilePath
        {
            get
            {
                return _currentFile.FullName;
            }
        }

        public IEnumerable<Property> Properties
        {
            get { return _properties; }
        }

        public IEnumerable<Input> Inputs
        {
            get { return _inputs; }
        }

        public IEnumerable<Scope> Scopes
        {
            get { return _scopes; }
        }

        private void Load(FileInfo file)
        {
            XDocument doc= XDocument.Load(file.FullName);

            Name = doc.Element("project").GetAttribute("name");
            _properties.AddRange(LoadProperties(doc));
            _inputs.AddRange(LoadInputs(doc));
            _scopes.AddRange(LoadScopes(doc));
        }

        private List<Scope> LoadScopes(XDocument doc)
        {
            var items = doc.GetNodes("project/scope")
                .Select(p => new Scope(p.GetSafeAttribute("version"), LoadBlocks(p)))
                .ToList();

            return items;
        }

        private List<Block> LoadBlocks(XElement scope)
        {
            var items = scope.GetNodes("block")
                .Select(p => new Block(p.GetAttribute("title"), LoadQuery(p),
                    bool.Parse(p.GetSafeAttribute("showOnEmpty", "true"))))
                .ToList();

            return items;
        }

        private Query LoadQuery(XElement block)
        {
            return new Query(block.GetNodeValue("query"));
        }

        private List<Input> LoadInputs(XDocument doc)
        {
            var items = doc.GetNodes("project/input/field")
                .Select(p => new Input(p.GetAttribute("name"),
                        p.GetAttribute("title"), 
                        p.GetSafeAttribute("type"), 
                        bool.Parse(p.GetSafeAttribute("allowBlank", "true"))))
                .ToList();

            return items;
        }

        private List<Property> LoadProperties(XDocument doc)
        {
            var items = doc.GetNodes("project/props/prop")
                .Select(p => new Property(p.GetAttribute("name"), 
                        p.GetAttribute("value")))
                .ToList();

            return items;
        }

        public static DaxDocument Load(String file)
        {
            return new DaxDocument(file);
        }

        public override String ToString()
        {
            return String.Format("Name={0}", Name);
        }
    }
}
