using dax.Db;
using dax.Document;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace dax.Gui
{
    public partial class TableControl : UserControl
    {
        private readonly Regex CLEAR_PATTERN = new Regex(@"[\t ]+");
        private readonly IQueryBlock _queryBlock;
        private readonly Block _block;
        private readonly INotificationView _notificationView;

        public TableControl(Block block, IQueryBlock queryBlock, INotificationView notificationView)
        {
            InitializeComponent();

            _block = block;
            _queryBlock = queryBlock;
            _notificationView = notificationView;
            Title = block.Title;
            labelTitle.ToolTip = CLEAR_PATTERN.Replace(queryBlock.QueryText, " ").Trim();
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

        private void EnableControls(bool enable)
        {
            labelLoading.Visibility = enable ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;
            buttonNext.IsEnabled = enable;
            buttonPrev.IsEnabled = enable;
        }

        private void NextPage()
        {
            EnableControls(false);
            Task task = _queryBlock.NextPageAsync();

            task.GetAwaiter().OnCompleted(() => OnOperationComplete(task));
        }

        private void PrevPage()
        {
            EnableControls(false);
            Task task = _queryBlock.PrevPageAsync();

            task.GetAwaiter().OnCompleted(() => OnOperationComplete(task));
        }

        private void OnOperationComplete(Task task)
        {
            if (task.Exception != null)
            {
                _notificationView.ShowError(task.Exception.GetBaseException().Message);
            }

            RefreshPaging();
            EnableControls(true);
        }

        private void RefreshPaging()
        {
            gridTable.ItemsSource = _queryBlock.Table.DefaultView;
            labelCurrentPage.Text = (_queryBlock.PageIndex + 1).ToString();
        }

        #region Event Handlers

        private void buttonPrev_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            PrevPage();
        }


        private void buttonNext_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            NextPage();
        }

        #endregion
    }
}
