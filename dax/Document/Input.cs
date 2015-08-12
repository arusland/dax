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

namespace dax.Document
{
    public class Input
    {
        public Input(String name, String title, InputType type, String defaultValue, bool allowBlank, bool enabled)
        {
            Name = name;
            Title = title;
            Type = type;
            DefaultValue = defaultValue;
            AllowBlank = allowBlank && (type != InputType.Date);
            Enabled = enabled;
        }

        public String Name
        {
            get;
            private set;
        }

        public String Title
        {
            get;
            private set;
        }

        public String DefaultValue
        {
            get;
            private set;
        }

        public bool AllowBlank
        {
            get;
            private set;
        }

        public bool Enabled
        {
            get;
            private set;
        }

        public InputType Type
        {
            get;
            private set;
        }

        public override String ToString()
        {
            return String.Format("Name={0}; Title={1}; Type={2}; AllowBlank={3}", Name, Title, Type, AllowBlank);
        }
    }
}
