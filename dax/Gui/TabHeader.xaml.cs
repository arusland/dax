using System;
using System.Windows.Controls;

namespace dax.Gui
{
    public partial class TabHeader : UserControl
    {
        public TabHeader(String documentName, String connection = null, String scopeVersion = null)
        {
            InitializeComponent();
            DocumentName = documentName;
            Connection = connection;
            ScopeVersion = scopeVersion;
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

        public String ScopeVersion
        {
            get
            {
                return labelScopeVersion.Text;
            }
            set
            {
                String version = value ?? String.Empty;

                if (!String.IsNullOrEmpty(version))
                {
                    version = String.Format("[db: {0}]", version);
                }

                labelScopeVersion.Text = version;
                labelScopeVersion.Visibility = String.IsNullOrEmpty(version) ?
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
