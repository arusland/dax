using dax.Document;
using dax.Gui;
using System.Windows;
using System.Windows.Controls;
using dax.Core;
using dax.Db;

namespace dax
{
    public partial class MainWindow : Window
    {
        private readonly TabItem _addnewTabItem;

        public MainWindow()
        {
            InitializeComponent();

            DaxManager daxManager = new DaxManager(ProviderFactory.Instance, @"d:\WORK\MyProjects\dax\template.xml");

            var item  = new TabItem();
            item.Header = daxManager.DocumentName;
            var tabItemControl = new TabDocumentControl(daxManager);
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
                item.Content = new TabDocumentControl(null);
                tabControlMain.Items.Insert(tabControlMain.Items.Count - 1, item);
                tabControlMain.SelectedItem = item;
            }
        }
    }
}
