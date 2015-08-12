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
using System.Windows.Media;

namespace dax.Gui
{
    public partial class BoolInputControl : BaseInputControl
    {
        private readonly Input _input;
        private static Brush _hightlightBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0xCB, 0xF5, 0xD6));

        public BoolInputControl(Input input)
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
            get { return false; }
        }

        public override String InputValue
        {
            get
            {
                return checkBoxValue.IsChecked == true ? "1" : "0";
            }
            set
            {
                checkBoxValue.IsChecked = "1" == value;
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
                return textBlockWrapper.Background == _hightlightBrush;
            }
            set
            {
                textBlockWrapper.Background = value ? _hightlightBrush : Brushes.Transparent;
            }
        }

        public override void Reset()
        {
            IsSelected = _input.Enabled; ;
            InputValue = _input.DefaultValue;
            IsHighlighted = false;
        }

        private void RefreshView()
        {
            if (checkBoxEnabled != null && checkBoxValue != null)
            {
                bool selected = true == checkBoxEnabled.IsChecked;
                checkBoxValue.IsEnabled = selected;
                checkBoxEnabled.Foreground = selected ? Brushes.Black : Brushes.Gray;
            }
        }

        #region Event Handlers

        private void CheckBoxEnabled_CheckedChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            RefreshView();
        }

        #endregion
    }
}
