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

        public ConnectionEditDialog(IProviderFactory providerFactory, INotificationView notificationView, Window owner)
        {
            InitializeComponent();
            _providerFactory = providerFactory;
            _notificationView = notificationView;
            Owner = owner;
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

        private void buttonTest_Click(object sender, RoutedEventArgs e)
        {
            IConnection connection = _providerFactory.NewConnection(textBoxServer.Text.Trim(), textBoxDatabase.Text.Trim(),
                textBoxLogin.Text.Trim(), textBoxPassword.SecurePassword.FromSecureString());

            buttonTest.IsEnabled = false;
            var task = connection.Test();

            task.GetAwaiter().OnCompleted(() =>
            {
                if (this.IsActive)
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
                }
            });
        }

        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            ServerName = textBoxServer.Text.Trim();
            DbName = textBoxDatabase.Text.Trim();
            Login = textBoxLogin.Text.Trim();
            Password = textBoxPassword.SecurePassword.FromSecureString();
            DialogResult = true;
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            textBoxServer.Text = ServerName;
            textBoxDatabase.Text = DbName;
            textBoxLogin.Text = Login;
            Password.ToSecureString(textBoxPassword.SecurePassword);
            textBoxServer.Focus();
        }
    }
}
