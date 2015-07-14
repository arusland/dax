using dax.Db;
using dax.Db.Connect;
using System;
using System.Linq;
using System.Windows;

namespace dax.Gui
{
    public partial class ConnectionsEditDialog : Window
    {
        private readonly IProviderFactory _providerFactory;
        private readonly IConnectionRepository _connectionRepository;
        private readonly INotificationView _notificationView;

        public ConnectionsEditDialog(IProviderFactory providerFactory, IConnectionRepository connectionRepository, INotificationView notificationView)
        {
            InitializeComponent();
            _providerFactory = providerFactory;
            _connectionRepository = connectionRepository;
            _notificationView = notificationView;
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            ReloadConnections();
        }

        private void ReloadConnections()
        {
            listConnections.Items.Clear();
            foreach (ListItem connection in _connectionRepository.Connections.Select(p => new ListItem(p)).OrderBy(p => p.ToString()))
            {
                listConnections.Items.Add(connection);
            }
        }

        private void buttonAdd_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ConnectionEditDialog(_providerFactory, _notificationView);

            if (dialog.ShowDialog() == true)
            {
                IConnection connection = _providerFactory.NewConnection(dialog.ServerName, dialog.DbName, dialog.Login, dialog.Password);

                _connectionRepository.Add(connection);
                ReloadConnections();
            }
        }

        private void buttonEdit_Click(object sender, RoutedEventArgs e)
        {
            var oldConnection = listConnections.SelectedItem as ListItem;

            if (oldConnection != null)
            {
                var dialog = new ConnectionEditDialog(_providerFactory, _notificationView)
                {
                    ServerName = oldConnection.Connection.ServerName,
                    DbName = oldConnection.Connection.DbName,
                    Login = oldConnection.Connection.Login,
                    Password = oldConnection.Connection.Password
                };

                if (dialog.ShowDialog() == true)
                {
                    IConnection newConnection = _providerFactory.NewConnection(dialog.ServerName, dialog.DbName, dialog.Login, dialog.Password);

                    _connectionRepository.Remove(oldConnection.Connection);
                    _connectionRepository.Add(newConnection);
                    ReloadConnections();
                }
            }
        }

        private void buttonDelete_Click(object sender, RoutedEventArgs e)
        {
            var oldConnection = listConnections.SelectedItem as ListItem;

            if (oldConnection != null)
            {
                if (_notificationView.ShowQuestion(String.Format("Are you sure to delete connection '{0}'?", oldConnection),
                    MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    _connectionRepository.Remove(oldConnection.Connection);
                    ReloadConnections();
                }
            }
        }

        private class ListItem
        {
            public ListItem(IConnection connection)
            {
                Connection = connection;
            }

            public IConnection Connection
            {
                get;
                private set;
            }

            public override string ToString()
            {
                return String.Format("{0}.{1}", Connection.ServerName, Connection.DbName);
            }
        }
    }
}
