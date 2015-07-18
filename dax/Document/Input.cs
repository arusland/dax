using System;

namespace dax.Document
{
    public class Input
    {
        private const String BOOL_TYPE = "bool";

        public Input(String name, String title, String type, bool allowBlank)
        {
            Name = name;
            Title = title;
            Type = type;
            AllowBlank = allowBlank;
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

        public String Type
        {
            get;
            private set;
        }

        public bool AllowBlank
        {
            get;
            private set;
        }

        public bool IsBoolType
        {
            get
            {
                return BOOL_TYPE == Type;
            }
        }

        public override String ToString()
        {
            return String.Format("Name={0}; Title={1}; Type={2}; AllowBlank={3}", Name, Title, Type, AllowBlank);
        }
    }
}
