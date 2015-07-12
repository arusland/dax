using dax.Db;
using dax.Document;
using System;
using System.Threading.Tasks;
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
            PrevPage();
        }

        private async Task PrevPage()
        {
            EnableControls(false);
            await _queryBlock.PrevPageAsync();
            RefreshPaging();
            EnableControls(true);
        }

        private void EnableControls(bool enable)
        {
            labelLoading.Visibility = enable ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;
            buttonNext.IsEnabled = enable;
            buttonPrev.IsEnabled = enable;
        }
        private void buttonNext_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            NextPage();
        }

        private async Task NextPage()
        {
            EnableControls(false); 
            await _queryBlock.NextPageAsync();
            RefreshPaging();
            EnableControls(true);
        }

        private void RefreshPaging()
        {
            gridTable.ItemsSource = _queryBlock.Table.DefaultView;
            labelCurrentPage.Text = (_queryBlock.PageIndex + 1).ToString();
        }
    }
}
