using dax.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using dax.Db.SqlServer;
using dax.Db;

namespace dax
{
    public partial class MainWindow : Window
    {
        private IQueryBlock block;
        public MainWindow()
        {
            InitializeComponent();
            //DaxDocument doc = DaxDocument.Load(@"d:\WORK\MyProjects\dax\template.xml");

            //Dictionary<string, object> map = new Dictionary<string, object>()
            //{
            //    {"ITEM_CODE", "xxx"}
            //};

            //new SqlServerProvider().Exec(null);

            block = ProviderFactory.Instance.Create().CreateBlock("select * from sys.objects");
            block.Update();
            gridProducts.ItemsSource = block.Table.DefaultView;
            textBoxCurrentPage.Text = (block.PageIndex + 1).ToString();
        }

        private void btnNextPage_Click(object sender, RoutedEventArgs e)
        {
            block.NextPage();
            gridProducts.ItemsSource = block.Table.DefaultView;
            textBoxCurrentPage.Text = (block.PageIndex + 1).ToString();
        }

        private void btnPrevPage_Click(object sender, RoutedEventArgs e)
        {
            block.PrevPage();
            gridProducts.ItemsSource = block.Table.DefaultView;
            textBoxCurrentPage.Text = (block.PageIndex + 1).ToString();
        }
    }
}
