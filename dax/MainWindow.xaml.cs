using dax.Core;
using dax.Db;
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
        private const String TITLE_TEMPLATE_WITH_DOCUMENT = "DAta eXplorer - {0} - ver{1}";
        private const String TITLE_TEMPLATE = "DAta eXplorer - ver{0}";
        private readonly TabItem _addnewTabItem;
        private readonly TabItem _aboutTabItem;
        private readonly TabItem _startUpTabItem;

        public MainWindow()
        {
            InitializeComponent();

            _startUpTabItem = new TabItem() { Header = "Home", Content = new StartUpPageControl() };
            tabControlMain.Items.Add(_startUpTabItem);
            _addnewTabItem = new TabItem() { Header = "+" };
            _addnewTabItem.MouseUp += TextBlockNewtab_MouseUp;
            tabControlMain.Items.Add(_addnewTabItem);
            _aboutTabItem = new TabItem() { Header = "About" };
            _aboutTabItem.MouseUp += TextBlockAbout_MouseUp;
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
                DaxManager daxManager = new DaxManager(ProviderFactory.Instance, filePath,
                        TaskScheduler.FromCurrentSynchronizationContext());

                var tabItemControl = new TabDocumentControl(daxManager, this);
                tabItemControl.OnCloseDocument += TabItemControl_OnCloseDocument;
                var item = new TabItem();
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

        private void RefreshTabsView()
        {
            _startUpTabItem.Visibility = CurrentDocumentTabItems.Any() ? Visibility.Collapsed : Visibility.Visible;

            if (_startUpTabItem.IsVisible)
            {
                tabControlMain.SelectedItem = _startUpTabItem;
            }
        }

        public void SetStatus(string text)
        {
            statusLabel.Content = text;
        }

        public void ShowError(string message)
        {
            MessageBox.Show(message, "DAX Error", MessageBoxButton.OK, MessageBoxImage.Error);
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

        #region Event Handlers

        private void tabControlMain_SelectionChanged(object sender, SelectionChangedEventArgs e)
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
            AboutBox.Show();
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

        #endregion
    }
}
