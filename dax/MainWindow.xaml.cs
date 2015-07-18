using dax.Core;
using dax.Core.Events;
using dax.Db;
using dax.Db.Connect;
using dax.Extensions;
using dax.Gui;
using dax.Managers;
using dax.Utils;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace dax
{
    public partial class MainWindow : Window, INotificationView
    {
        private const String TITLE_TEMPLATE_WITH_DOCUMENT = "DAta eXplorer - [{0}] - ver {1}";
        private const String TITLE_TEMPLATE = "DAta eXplorer - ver {0}";
        private const String TITLE_MESSAGE_BOX = "Data Explorer";

        private readonly TabItem _addnewTabItem;
        private readonly TabItem _aboutTabItem;
        private readonly TabItem _startUpTabItem;
        private readonly ConnectionRepository _connectionRepository;
        private readonly IProviderFactory _providerFactory;
        private readonly Dictionary<DaxManager, IDbProvider> _providerMapping = new Dictionary<DaxManager, IDbProvider>();
        private readonly FileWatcher _fileWathcer;

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
            _fileWathcer = new FileWatcher(TaskScheduler.FromCurrentSynchronizationContext());

            _fileWathcer.OnFileChanged += FileWatcher_OnFileChanged;
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
                var item = new TabItem();
                item.Tag = daxManager;
                var tabHeader = new TabHeader(daxManager.DocumentName);
                tabHeader.OnClose += TabHeader_OnClose;
                item.Header = tabHeader;
                item.Content = tabItemControl;

                // add new tab just before adding tab
                int index = tabControlMain.Items.IndexOf(_addnewTabItem) - 1;
                tabControlMain.Items.Insert(index, item);
                tabItemControl.ReloadDocument();
                RefreshTabsView();
                tabControlMain.SelectedValue = item;

                _fileWathcer.WatchFile(filePath);
            }
            catch (System.Exception ex)
            {
                ShowError(ex.Message);
                RefreshTabsView();
            }
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
                    ((TabHeader)currentTabItem.Header).Connection = provider.Connection.Format();
                }
            }
        }

        private void AddHotKeys()
        {
            RoutedCommand openDocCmd = new RoutedCommand();
            openDocCmd.InputGestures.Add(new KeyGesture(Key.O, ModifierKeys.Control));
            CommandBindings.Add(new CommandBinding(openDocCmd, OpenNewDocumentHandler));

            RoutedCommand searchCmd = new RoutedCommand();
            searchCmd.InputGestures.Add(new KeyGesture(Key.F5));
            CommandBindings.Add(new CommandBinding(searchCmd, SearchHandler));

            RoutedCommand cancelSearchCmd = new RoutedCommand();
            cancelSearchCmd.InputGestures.Add(new KeyGesture(Key.Escape));
            CommandBindings.Add(new CommandBinding(cancelSearchCmd, CancelSearchHandler));
        }

        private void OpenNewDocumentCommand()
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

        private void TextBlockAbout_MouseUp(object sender, MouseButtonEventArgs e)
        {
            AboutBox.Show(this);
            SelectFirstDocument();
        }

        private void OpenNewDocumentHandler(object sender, ExecutedRoutedEventArgs e)
        {
            OpenNewDocumentCommand();
        }

        private void CancelSearchHandler(object sender, ExecutedRoutedEventArgs e)
        {
            var currentTabItem = (TabItem)tabControlMain.SelectedItem;

            if (currentTabItem != null)
            {
                var doc = currentTabItem.Content as TabDocumentControl;

                if (doc != null)
                {
                    doc.InvokeCancelSearch();
                }
            }
        }

        private void SearchHandler(object sender, ExecutedRoutedEventArgs e)
        {
            var currentTabItem = (TabItem)tabControlMain.SelectedItem;

            if (currentTabItem != null)
            {
                var doc = currentTabItem.Content as TabDocumentControl;

                if (doc != null)
                {
                    doc.InvokeSearch();
                }
            }
        }

        private void TextBlockNewtab_MouseUp(object sender, MouseButtonEventArgs e)
        {
            OpenNewDocumentCommand();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            _connectionRepository.Save();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _connectionRepository.Load();
            AddHotKeys();
        }

        private void TabHeader_OnClose(object sender, EventArgs e)
        {
            var tabItem = CurrentDocumentTabItems.FirstOrDefault(p => p.Header == sender);

            if (tabItem != null)
            {
                tabControlMain.Items.Remove(tabItem);
                var closedFile = ((DaxManager)tabItem.Tag).FilePath;

                if (CurrentDocuments.All(p => p.Manager.FilePath != closedFile))
                {
                    _fileWathcer.UnwatchFile(closedFile);
                }
            }
        }

        private void FileWatcher_OnFileChanged(object sender, FileChangedEventArgs e)
        {
            if (ShowQuestion(String.Format("File '{0}' was modified. Reload it?", e.FilePath), MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                var documentsToReload = CurrentDocuments
                    .Where(p => p.Manager.FilePath.ToLower() == e.FilePath.ToLower())
                    .ToList();

                foreach (var doc in documentsToReload)
                {
                    doc.ReloadFile();
                }
            }
        }

        #endregion
    }
}
