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
            IsSelected = _input.Enabled; ;
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
