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
