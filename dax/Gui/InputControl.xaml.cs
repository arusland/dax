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
using System.Windows.Input;
using System.Windows.Media;

namespace dax.Gui
{
    public partial class InputControl : BaseInputControl
    {
        private readonly Input _input;
        private readonly static Brush SELECTED_BRUSH = new SolidColorBrush(Color.FromRgb(0xCB, 0xF5, 0xD6));
        public override event EventHandler<EventArgs> OnSubmit;

        public InputControl(Input input)
        {
            InitializeComponent();
            _input = input;
            InputTitle = _input.Title;
            InputValue = _input.DefaultValue;
            IsSelected = _input.Enabled;
            RefreshView();
        }

        public override bool IsSelected
        {
            get
            {
                return checkBoxEnabled.IsChecked == true;
            }
            set
            {
                checkBoxEnabled.IsChecked = value;
            }
        }

        public override bool IsBlank
        {
            get
            {
                return String.IsNullOrEmpty(InputValue);
            }
        }

        public override String InputValue
        {
            get
            {
                return textBoxValue.Text;
            }
            set
            {
                textBoxValue.Text = value;
            }
        }

        public override String InputName
        {
            get
            {
                return _input.Name;
            }
        }

        public override String InputTitle
        {
            get
            {
                return labelName.Text;
            }
            protected set
            {
                labelName.Text = value;
            }
        }

        public override Input Context
        {
            get
            {
                return _input;
            }
        }

        public override bool IsHighlighted
        {
            get
            {
                return textBoxValue.Background == SELECTED_BRUSH;
            }
            set
            {
                textBoxValue.Background = value ? SELECTED_BRUSH : Brushes.Transparent;
            }
        }

        public override void Reset()
        {
            IsSelected = _input.Enabled;
            InputValue = _input.DefaultValue;
            IsHighlighted = false;
        }

        private void RefreshView()
        {
            if (checkBoxEnabled != null && textBoxValue != null)
            {
                bool selected = true == checkBoxEnabled.IsChecked;
                textBoxValue.IsEnabled = selected;
                checkBoxEnabled.Foreground = selected ? Brushes.Black : Brushes.Gray;
            }
        }

        #region Event Handlers

        private void CheckBoxEnabled_CheckedChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            RefreshView();

            if (checkBoxEnabled != null && textBoxValue != null)
            {
                if (true == checkBoxEnabled.IsChecked)
                {
                    textBoxValue.SelectAll();
                    textBoxValue.Focus();
                }
            }
        }

        private void TextBoxValue_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter && OnSubmit != null)
            {
                OnSubmit(this, EventArgs.Empty);
            }
        }

        #endregion
    }
}
