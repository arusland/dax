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
using dax.Document;
using System;

namespace dax.Core.Events
{
    public class NewBlockAddedEventArgs : EventArgs
    {
        public NewBlockAddedEventArgs(Block block, IQueryBlock queryBlock)
        {
            Block = block;
            QueryBlock = queryBlock;
        }

        public Block Block
        {
            get;
            private set;
        }

        public IQueryBlock QueryBlock
        {
            get;
            private set;
        }

        public bool Canceled
        {
            get;
            set;
        }
    }
}
