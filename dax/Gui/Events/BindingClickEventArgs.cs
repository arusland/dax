using dax.Document;
using System;

namespace dax.Gui.Events
{
    public class BindingClickEventArgs : EventArgs
    {
        public BindingClickEventArgs(Binding binding, String value)
        {
            Binding = binding;
            Value = value;
        }        

        public Binding Binding
        {
            get;
            private set;
        }

        public String Value
        {
            get;
            private set;
        }
    }
}
