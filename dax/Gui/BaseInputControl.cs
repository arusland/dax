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
