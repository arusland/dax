using dax.Document;
using System;
using System.Windows.Media;

namespace dax.Gui
{
    public partial class DateInputControl : BaseInputControl
    {
        private const String SQL_DATE_FORMAT = "yyyy-MM-dd HH:mm:ss";
        private readonly Input _input;
        private static Brush _hightlightBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0xCB, 0xF5, 0xD6));

        public DateInputControl(Input input)
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
            get { return datePicker.SelectedDate == null; }
        }

        public override String InputValue
        {
            get
            {
                return IsBlank ? String.Empty : "'" + datePicker.SelectedDate.Value.ToString(SQL_DATE_FORMAT) + "'";
            }
            set
            {
                DateTime dt;

                if (!String.IsNullOrEmpty(value))
                {
                    value = value.Trim('\'');

                    if (DateTime.TryParseExact(value, SQL_DATE_FORMAT, System.Threading.Thread.CurrentThread.CurrentCulture,
                        System.Globalization.DateTimeStyles.None, out dt))
                    {
                        datePicker.SelectedDate = dt;
                    }
                }
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
                return datePicker.Background == _hightlightBrush;
            }
            set
            {
                datePicker.Background = value ? _hightlightBrush : Brushes.Transparent;
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
            if (checkBoxEnabled != null && datePicker != null)
            {
                bool selected = true == checkBoxEnabled.IsChecked;
                datePicker.IsEnabled = selected;
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
