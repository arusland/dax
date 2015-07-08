using System;

namespace dax.Core.Document
{
    public class Input
    {
        public Input(String name, String title, String type, String bind)
        {
            Name = name;
            Title = title;
            Type = type;
            Bind = bind;
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

        public String Bind
        {
            get;
            private set;
        }

        public override String ToString()
        {
            return String.Format("Name={0}; Title={1}; Type={2}; Bind={3}", Name, Title, Type, Bind);
        }
    }
}
