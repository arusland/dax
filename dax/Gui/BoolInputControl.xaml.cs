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
                checkBoxValue.IsEnabled = checkBoxEnabled.IsChecked == true;
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
