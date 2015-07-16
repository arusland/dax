using System;
using System.Windows.Controls;

namespace dax.Gui
{
    public partial class TabHeader : UserControl
    {
        public TabHeader(String documentName, String connection = null)
        {
            InitializeComponent();
            DocumentName = documentName;
            Connection = connection;
        }

        public String DocumentName
        {
            get
            {
                return labelName.Text;
            }
            set
            {
                labelName.Text = value;
            }
        }

        public String Connection
        {
            get
            {
                return labelConnection.Text;
            }
            set
            {
                labelConnection.Text = value ?? String.Empty;
                labelConnection.Visibility = String.IsNullOrEmpty(labelConnection.Text) ?
                    System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible; 
            }
        }

        public event EventHandler<EventArgs> OnClose;

        private void ButtonClose_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (OnClose != null)
            {
                OnClose(this, EventArgs.Empty);
            }
        }
    }
}
