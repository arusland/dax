using dax.Db;
using dax.Document;
using System;
using System.Windows.Controls;

namespace dax.Gui
{
    public partial class TableControl : UserControl
    {
        private readonly IQueryBlock _queryBlock;
        private readonly Block _block;

        public TableControl(Block block, IQueryBlock queryBlock)
        {
            InitializeComponent();

            _block = block;
            _queryBlock = queryBlock;
            Title = block.Title;
            RefreshPaging();
        }

        public String Title
        {
            get
            {
                return labelTitle.Text;
            }

            private set
            {
                labelTitle.Text = value;
            }
        }

        private void buttonPrev_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _queryBlock.PrevPage();
            RefreshPaging();
        }

        private void buttonNext_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _queryBlock.NextPage();
            RefreshPaging();            
        }

        private void RefreshPaging()
        {
            gridTable.ItemsSource = _queryBlock.Table.DefaultView;
            labelCurrentPage.Text = (_queryBlock.PageIndex + 1).ToString();
        }
    }
}
