using dax.Core;
using dax.Core.Events;
using dax.Db;
using dax.Db.Connect;
using dax.Extensions;
using dax.Gui;
using dax.Utils;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace dax
{
    public partial class MainWindow : Window, INotificationView
    {
        private const String TITLE_TEMPLATE_WITH_DOCUMENT = "DAta eXplorer - [{0}] - ver{1}";
        private const String TITLE_TEMPLATE = "DAta eXplorer - ver{0}";
        private const String TITLE_MESSAGE_BOX = "Data Explorer";
        private const String TAB_ITEM_HEADER = "{0} [{1}]";
        private readonly TabItem _addnewTabItem;
        private readonly TabItem _aboutTabItem;
        private readonly TabItem _startUpTabItem;
        private readonly ConnectionRepository _connectionRepository;
        private readonly IProviderFactory _providerFactory;
        private readonly Dictionary<DaxManager, IDbProvider> _providerMapping = new Dictionary<DaxManager, IDbProvider>();

        public MainWindow()
        {
            InitializeComponent();

            _providerFactory = ProviderFactory.Instance;
            _connectionRepository = new ConnectionRepository(ProviderFactory.Instance);
            _startUpTabItem = new TabItem() { Header = "Home", Content = new StartUpPageControl() };
            tabControlMain.Items.Add(_startUpTabItem);
            _addnewTabItem = new TabItem() { Header = "+" };
            _addnewTabItem.Content = new BackgroundControl();
            _addnewTabItem.MouseUp += TextBlockNewtab_MouseUp;
            tabControlMain.Items.Add(_addnewTabItem);
            _aboutTabItem = new TabItem() { Header = "About" };
            _aboutTabItem.MouseUp += TextBlockAbout_MouseUp;
            _aboutTabItem.Content = new BackgroundControl();
            tabControlMain.Items.Add(_aboutTabItem);
        }

        private IEnumerable<TabItem> CurrentDocumentTabItems
        {
            get
            {
                return tabControlMain.Items.Cast<TabItem>()
                    .Where(p => p.Content is TabDocumentControl);
            }
        }

        private IEnumerable<TabDocumentControl> CurrentDocuments
        {
            get
            {
                return CurrentDocumentTabItems.Select(p => p.Content as TabDocumentControl);
            }
        }

        private void OpenDocument(String filePath)
        {
            try
            {
                DaxManager daxManager = new DaxManager(filePath, TaskScheduler.FromCurrentSynchronizationContext());
                daxManager.OnQueryProvider += DaxManager_OnQueryProvider;

                var tabItemControl = new TabDocumentControl(daxManager, this);
                tabItemControl.OnCloseDocument += TabItemControl_OnCloseDocument;
                var item = new TabItem();
                item.Tag = daxManager;
                item.Header = daxManager.DocumentName;
                item.Content = tabItemControl;

                // add new tab just before adding tab
                int index = tabControlMain.Items.IndexOf(_addnewTabItem) - 1;
                tabControlMain.Items.Insert(index, item);
                tabItemControl.ReloadDocument();
                RefreshTabsView();
                tabControlMain.SelectedValue = item;
            }
            catch (System.Exception ex)
            {
                ShowError(ex.Message);
                RefreshTabsView();
            }
        }


        private void DaxManager_OnQueryProvider(object sender, QueryProviderEventArgs e)
        {
            if (_providerMapping.ContainsKey(e.Manager) && !e.Reset)
            {
                e.Provider = _providerMapping[e.Manager];
            }
            else
            {
                var dialog = new ConnectionsEditDialog(ProviderFactory.Instance, _connectionRepository, this, this);

                if (dialog.ShowDialog() == true && dialog.SelectedConnection != null)
                {
                    var provider = _providerFactory.Create(dialog.SelectedConnection);
                    _providerMapping[e.Manager] = provider;
                    e.Provider = provider;
                }
            }

            UpdateCurrentTabHeader();
        }

        private void RefreshTabsView()
        {
            _startUpTabItem.Visibility = CurrentDocumentTabItems.Any() ? Visibility.Collapsed : Visibility.Visible;

            if (_startUpTabItem.IsVisible)
            {
                tabControlMain.SelectedItem = _startUpTabItem;
            }
        }

        private void SelectFirstDocument()
        {
            var currentTab = CurrentDocumentTabItems.FirstOrDefault();

            if (currentTab != null)
            {
                tabControlMain.SelectedItem = currentTab;
            }
            else
            {
                RefreshTabsView();
            }
        }

        private void UpdateMainTitle()
        {
            if (tabControlMain.SelectedItem != null)
            {
                var doc = ((TabItem)tabControlMain.SelectedItem).Content as TabDocumentControl;

                if (doc != null)
                {
                    this.Title = String.Format(TITLE_TEMPLATE_WITH_DOCUMENT, doc.DocumentTitle, VersionUtils.GetVersion());
                }
                else
                {
                    this.Title = String.Format(TITLE_TEMPLATE, VersionUtils.GetVersion());
                }
            }
        }

        private void UpdateCurrentTabHeader()
        {
            var currentTabItem = (TabItem)tabControlMain.SelectedItem;

            if (currentTabItem != null)
            {
                var doc = currentTabItem.Content as TabDocumentControl;

                if (_providerMapping.ContainsKey(doc.Manager))
                {
                    var provider = _providerMapping[doc.Manager];
                    currentTabItem.Header = String.Format(TAB_ITEM_HEADER, doc.Manager.DocumentName, provider.Connection.Format());
                }
            }
        }

        #region INotificationView interface

        public void SetStatus(string text)
        {
            statusLabel.Content = text;
        }

        public void ShowWarning(string message)
        {
            MessageBox.Show(message, TITLE_MESSAGE_BOX, MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        public void ShowMessage(string message)
        {
            MessageBox.Show(message, TITLE_MESSAGE_BOX, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public MessageBoxResult ShowQuestion(string message, MessageBoxButton buttons)
        {
            return MessageBox.Show(message, TITLE_MESSAGE_BOX, buttons, MessageBoxImage.Question);
        }

        public void ShowError(string message)
        {
            MessageBox.Show(message, TITLE_MESSAGE_BOX, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        #endregion

        #region Event Handlers

        private void TabControlMain_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tabControlMain.SelectedItem != null)
            {
                UpdateMainTitle();
            }
            else
            {
                SelectFirstDocument();
            }
        }

        private void TabItemControl_OnCloseDocument(object sender, EventArgs e)
        {
            var tabItem = CurrentDocumentTabItems.FirstOrDefault(p => p.Content == sender);

            if (tabItem != null)
            {
                tabControlMain.Items.Remove(tabItem);
            }
        }

        private void TextBlockAbout_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            AboutBox.Show(this);
            SelectFirstDocument();
        }

        private void TextBlockNewtab_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            if (openFileDialog.ShowDialog() == true)
            {
                OpenDocument(openFileDialog.FileName);
            }
            else
            {
                SelectFirstDocument();
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            _connectionRepository.Save();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _connectionRepository.Load();
        }

        #endregion
    }
}
