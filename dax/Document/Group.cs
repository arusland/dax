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
