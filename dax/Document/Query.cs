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

namespace dax.Document
{
    public abstract class Query
    {
        public abstract String Content
        {
            get;
            protected set;
        }

        public abstract bool Conditional
        {
            get;
            protected set;
        }

        public abstract IEnumerable<String> Variables
        {
            get;
        }

        public abstract String BuildQuery(Dictionary<String, String> inputValues);

        public abstract bool CanExecute(Dictionary<String, String> inputValues);

        public override String ToString()
        {
            return String.Format("Conent={0}", Content);
        }

        public static Query NewQuery(String content, bool conditional, bool skipWhenNoInput)
        {
            if (QueryContainer.IsAcceptable(content))
            {
                return new QueryContainer(content, skipWhenNoInput);
            }

            return new QueryBasic(content, conditional);
        }
    }
}
