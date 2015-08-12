/*
 * Copyright 2015 the original author or authors.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

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
            XDocument doc = XDocument.Load(file.FullName);

            Name = doc.Element("project").GetAttribute("name");
            _properties.AddRange(LoadProperties(doc));
            _inputs.AddRange(LoadInputs(doc));
            _scopes.AddRange(LoadScopes(doc));
        }

        private List<Scope> LoadScopes(XDocument doc)
        {
            var scopes = doc.GetNodes("project/scope")
                .Select(p => new Scope(p.GetSafeAttribute("version"), LoadBlocks(p)))
                .ToList();

            return scopes;
        }

        private List<Block> LoadBlocks(XElement scope)
        {
            int index = 0;
            var blocks = scope.GetNodes("block")
                .Select(p => new Block(p.GetAttribute("title"), LoadQuery(p, bool.Parse(p.GetSafeAttribute("skipWhenNoInput", "false"))),
                            LoadBindings(p), LoadGroups(p.GetSafeAttribute("groups", String.Empty)),
                            bool.Parse(p.GetSafeAttribute("showOnEmpty", "true")), index++))
                .ToList();

            return blocks;
        }

        private List<Binding> LoadBindings(XElement block)
        {
            var binds = block.GetNodes("bindings/bind")
                .Select(p => new Binding(p.GetAttribute("column"), p.GetAttribute("field")))
                .ToList();

            return binds;
        }

        private Query LoadQuery(XElement block, bool skipWhenNoInput)
        {
            return Query.NewQuery(block.GetNodeValue("query"), false, skipWhenNoInput);
        }

        private List<Group> LoadGroups(String groups)
        {
            return groups.Split(',')
                .Select(p => p.Trim())
                .Where(p => !String.IsNullOrEmpty(p))
                .Distinct()
                .Select(p => new Group(p))
                .ToList();
        }

        private List<Input> LoadInputs(XDocument doc)
        {
            var items = doc.GetNodes("project/input/field")
                .Select(p => new Input(p.GetAttribute("name"),
                        p.GetAttribute("title"),
                        ParseInputType(p.GetSafeAttribute("type")),
                        p.GetSafeAttribute("default", ""),
                        bool.Parse(p.GetSafeAttribute("allowBlank", "false")),
                        bool.Parse(p.GetSafeAttribute("enabled", "true"))))
                .ToList();

            return items;
        }

        private static InputType ParseInputType(String type)
        {
            switch (type)
            {
                case "bool":
                    return InputType.Bool;
                case "date":
                    return InputType.Date;
                default:
                    return InputType.Text;
            }
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
