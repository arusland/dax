using dax.Core.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace dax
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DaxDocument doc = DaxDocument.Load(@"d:\WORK\MyProjects\dax\template.xml");

            Dictionary<string, object> map = new Dictionary<string, object>()
            {
                {"ITEM_CODE", "xxx"}
            };

            String query = doc.Scopes.First()
                .Blocks.First()
                .Query
                .BuildQuery(map);
        }
    }
}
