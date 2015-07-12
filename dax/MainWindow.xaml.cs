using dax.Core;
using dax.Db;
using dax.Gui;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace dax
{
    public partial class MainWindow : Window, INotificationView
    {
        private readonly TabItem _addnewTabItem;

        public MainWindow()
        {
            InitializeComponent();

            DaxManager daxManager = new DaxManager(ProviderFactory.Instance, @"d:\WORK\MyProjects\dax\template.xml", 
                TaskScheduler.FromCurrentSynchronizationContext());

            var item  = new TabItem();
            item.Header = daxManager.DocumentName;
            var tabItemControl = new TabDocumentControl(daxManager, this);
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

                this.Title = String.Format("DAx eXplorer - {0}", doc.DocumentTitle);
            }
        }

        public void SetStatus(string text)
        {
            statusLabel.Content = text;
        }
    }
}
