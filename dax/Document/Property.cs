using System;

namespace dax.Document
{
    public class Property
    {
        public Property(String name, String value)
        {
            Name = name;
            Value = value;
        }

        public String Name
        {
            get;
            private set;
        }

        public String Value
        {
            get;
            private set;
        }
                
        public override String ToString()
        {
            return String.Format("Name={0}; Value={1}", Name, Value);
        }
    }
}
