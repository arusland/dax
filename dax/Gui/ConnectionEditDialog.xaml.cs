using dax.Db;
using dax.Db.Connect;
using dax.Extensions;
using System;
using System.Windows;

namespace dax.Gui
{
    public partial class ConnectionEditDialog : Window
    {
        private readonly IProviderFactory _providerFactory;
        private readonly INotificationView _notificationView;

        public ConnectionEditDialog(IProviderFactory providerFactory, INotificationView notificationView)
        {
            InitializeComponent();
            _providerFactory = providerFactory;
            _notificationView = notificationView;
        }

        public String ServerName
        {
            get;
            set;
        }

        public String DbName
        {
            get;
            set;
        }

        public String Login
        {
            get;
            set;
        }

        public String Password
        {
            get;
            set;
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);

            textBoxServer.Text = ServerName;
            textBoxDatabase.Text = DbName;
            textBoxLogin.Text = Login;
            Password.ToSecureString(textBoxPassword.SecurePassword);
        }

        private void buttonTest_Click(object sender, RoutedEventArgs e)
        {
            IConnection connection = _providerFactory.NewConnection(textBoxServer.Text.Trim(), textBoxDatabase.Text.Trim(),
                textBoxLogin.Text.Trim(), textBoxPassword.SecurePassword.ToString());

            buttonTest.IsEnabled = false;
            var task = connection.Test();
            task.ContinueWith(p =>
            {
                buttonTest.IsEnabled = true;

                if (String.IsNullOrWhiteSpace(task.Result))
                {
                    _notificationView.ShowMessage("Connection is OK!");
                }
                else
                {
                    _notificationView.ShowWarning(task.Result);
                }
            });
        }

        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            ServerName = textBoxServer.Text.Trim();
            DbName = textBoxDatabase.Text.Trim();
            Login = textBoxLogin.Text.Trim();
            Password = textBoxPassword.SecurePassword.ToString();
            DialogResult = true;
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
