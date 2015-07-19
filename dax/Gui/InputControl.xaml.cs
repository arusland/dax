using dax.Document;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace dax.Gui
{
    public partial class InputControl : UserControl
    {
        private readonly Input _input;
        public event EventHandler<EventArgs> OnSubmit;

        public InputControl(Input input)
        {
            InitializeComponent();
            _input = input;
            InputTitle = _input.Title;
            InputValue = _input.DefaultValue;
            RefreshView();
        }

        public bool IsSelected
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

        public String InputValue
        {
            get
            {
                if (_input.IsBoolType)
                {
                    return checkBoxValue.IsChecked == true ? "1" : "0";
                }

                return textBoxValue.Text;
            }
            set
            {
                if (_input.IsBoolType)
                {
                    checkBoxValue.IsChecked = "1" == value;
                }
                else
                {
                    textBoxValue.Text = value;
                }
            }
        }

        public String InputName
        {
            get
            {
                return _input.Name;
            }
        }

        public String InputTitle
        {
            get
            {
                return labelName.Text;
            }
            private set
            {
                labelName.Text = value;
            }
        }

        public Input Context
        {
            get
            {
                return _input;
            }
        }

        private void RefreshView()
        {
            if (checkBoxEnabled != null && textBoxValue != null)
            {
                checkBoxValue.IsEnabled =
                textBoxValue.IsEnabled = checkBoxEnabled.IsChecked == true;
                textBoxValue.Visibility = _input.IsBoolType ? Visibility.Collapsed : Visibility.Visible;
                checkBoxValue.Visibility = _input.IsBoolType ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        #region Event Handlers

        private void checkBoxEnabled_CheckedChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            RefreshView();
        }

        private void textBoxValue_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter && OnSubmit != null)
            {
                OnSubmit(this, EventArgs.Empty);
            }
        }

        #endregion
    }
}
