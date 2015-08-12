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
    public class Group
    {
        public static readonly Group All = new Group("[All]");

        public Group(String name)
        {
            Name = name;
        }

        public String Name
        {
            get;
            private set;
        }

        public bool IsAll
        {
            get
            {
                return this.Equals(Group.All);
            }
        }

        public override bool Equals(object obj)
        {
            Group rhs = obj as Group;

            if (rhs == null)
            {
                return false;
            }

            return Name.Equals(rhs.Name);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public override string ToString()
        {
            return String.Format("Name={0}", Name);
        }
    }
}
