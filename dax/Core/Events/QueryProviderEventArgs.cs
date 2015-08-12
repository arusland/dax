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

using dax.Db;
using System;

namespace dax.Core.Events
{
    public class QueryProviderEventArgs : EventArgs
    {
        public QueryProviderEventArgs(DaxManager manager, bool reset = false)
        {
        	Manager = manager;
            Reset = reset;
        }
        
        public IDbProvider Provider
        {
            get;
            set;
        }

        public DaxManager Manager
        {
            get;
            private set;
        }

        public bool Reset
        {
            get;
            private set;
        }
    }
}
