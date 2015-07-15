using dax.Document;
using System;
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
            RefreshView();
        }

        public bool IsSelected
        {
            get
            {
                return checkBoxEnabled.IsChecked == true;
            }
        }

        public String InputValue
        {
            get
            {
                return textBoxValue.Text;
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
                return checkBoxEnabled.Content.ToString();
            }
            private set
            {
                checkBoxEnabled.Content = value;
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
                textBoxValue.IsEnabled = checkBoxEnabled.IsChecked == true;
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
