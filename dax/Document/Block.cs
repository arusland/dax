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

using System;
using System.Collections.Generic;
using System.Linq;

namespace dax.Document
{
    public class Block
    {
        public Block(String title, Query query, IEnumerable<Binding> bindings, IEnumerable<Group> groups, bool showOnEmpty, int order)
        {
            Title = title;
            Query = query;
            ShowOnEmpty = showOnEmpty;
            Order = order;
            Bindings = bindings.ToList();
            Groups = groups.ToList();
        }

        public String Title
        {
            get;
            private set;
        }

        public bool ShowOnEmpty
        {
            get;
            private set;
        }

        public Query Query
        {
            get;
            private set;
        }

        public int Order
        {
            get;
            private set;
        }

        public IEnumerable<Binding> Bindings
        {
            get;
            private set;
        }

        public IEnumerable<String> Variables
        {
            get { return Query.Variables; }
        }

        public IEnumerable<Group> Groups { get; private set; }

        public bool HasGroup(Group group)
        {
            return Groups.Any(p => p.Equals(group));
        }

        public bool CanExecute(Dictionary<String, String> inputValues)
        {
            return Query.CanExecute(inputValues);
        }

        public String BuildQuery(Dictionary<String, String> inputValues)
        {
            return Query.BuildQuery(inputValues);
        }

        public override String ToString()
        {
            return String.Format("Title={0}", Title);
        }

    }
}
