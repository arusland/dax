using System;

namespace dax.Document
{
    public class Binding
    {
        public Binding(String column, String field)
        {
            Column = column;
            Field = field;
        }

        public String Column
        {
            get;
            private set;
        }

        public String Field
        {
            get;
            private set;
        }
    }
}
