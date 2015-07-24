using dax.Document;
using System;
using System.Windows.Controls;

namespace dax.Gui
{
    public abstract class BaseInputControl : UserControl
    {
        public virtual event EventHandler<EventArgs> OnSubmit;

        public abstract String InputValue
        {
            get;
            set;
        }

        public abstract bool IsSelected
        {
            get;
            set;
        }

        public abstract bool IsBlank
        {
            get;
        }

        public abstract String InputName
        {
            get;
        }

        public abstract String InputTitle
        {
            get;
            protected set;
        }

        public abstract Input Context
        {
            get;
        }

        public abstract bool IsHighlighted
        {
            get;
            set;
        }

        public abstract void Reset();

        public static BaseInputControl Create(Input input)
        {
            switch (input.Type)
            {
                case InputType.Bool:
                    return new BoolInputControl(input);
                case InputType.Date:
                    return new DateInputControl(input);
                case InputType.Text:
                    return new InputControl(input);
                default:
                    throw new InvalidOperationException("Unsupported type: " + input.Type);
            }
        }
    }
}
