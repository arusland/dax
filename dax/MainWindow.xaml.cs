using dax.Core;
using dax.Db;
using dax.Gui;
using dax.Utils;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace dax
{
    public partial class MainWindow : Window, INotificationView
    {
        private const String TITLE_TEMPLATE = "DAx eXplorer - {0} - ver{1}";
        private readonly TabItem _addnewTabItem;

        public MainWindow()
        {
            InitializeComponent();

            DaxManager daxManager = new DaxManager(ProviderFactory.Instance, @"d:\WORK\MyProjects\dax\template.xml", 
                TaskScheduler.FromCurrentSynchronizationContext());
                        
            var tabItemControl = new TabDocumentControl(daxManager, this);
            var item = new TabItem();
            item.Header = daxManager.DocumentName;
            item.Content = tabItemControl;
            tabControlMain.Items.Add(item);
            tabItemControl.ReloadDocument();

            _addnewTabItem = new TabItem()
            {
                Header = "+"
            };
            tabControlMain.Items.Add(_addnewTabItem);
        }

        private void tabControlMain_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tabControlMain.SelectedItem == _addnewTabItem)
            {
                var item = new TabItem();
                item.Header = "new Document";
                item.Content = new TabDocumentControl(null, this);
                tabControlMain.Items.Insert(tabControlMain.Items.Count - 1, item);
                tabControlMain.SelectedItem = item;
            }
            else
            {
                var doc = ((TabItem)tabControlMain.SelectedItem).Content as TabDocumentControl;

                this.Title = String.Format(TITLE_TEMPLATE, doc.DocumentTitle, VersionUtils.GetVersion());
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
    }
}
