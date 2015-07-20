using dax.Db;
using dax.Gui.Events;
using System;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace dax.Gui
{
    public partial class TableControl : UserControl
    {
        private readonly Regex CLEAR_PATTERN = new Regex(@"[\t ]+");
        private readonly IQueryBlock _queryBlock;
        private readonly dax.Document.Block _block;
        private readonly INotificationView _notificationView;

        public TableControl(dax.Document.Block block, IQueryBlock queryBlock, INotificationView notificationView)
        {
            InitializeComponent();

            _block = block;
            _queryBlock = queryBlock;
            _notificationView = notificationView;
            Title = block.Title;
            labelTitle.ToolTip = CLEAR_PATTERN.Replace(_queryBlock.QueryText, " ").Trim();
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
            textBoxCurrentPage.IsEnabled = enable;
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
            textBoxCurrentPage.Text = (_queryBlock.PageIndex + 1).ToString();
        }

        #region Event Handlers

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            var menuCopySQL = new ContextMenu();
            var menuItemCopySQL = new MenuItem() { Header = "Copy SQL" };
            menuItemCopySQL.Click += MenuItemCopySQL_Click;
            menuCopySQL.Items.Add(menuItemCopySQL);
            labelTitle.ContextMenu = menuCopySQL;

            var menuCopySelectedCell = new ContextMenu();
            var menuItemCopySelectedCell = new MenuItem() { Header = "Copy Value" };
            menuCopySelectedCell.Items.Add(menuItemCopySelectedCell);
            menuItemCopySelectedCell.Click += MenuItemCopyValue_Click;

            gridTable.ContextMenu = menuCopySelectedCell;
        }

        private void MenuItemCopyValue_Click(object sender, RoutedEventArgs e)
        {
            DataRowView dataRow = (DataRowView)gridTable.SelectedItem;

            if (dataRow != null && gridTable.CurrentCell != null)
            {
                int index = gridTable.CurrentCell.Column.DisplayIndex;
                string cellValue = dataRow.Row.ItemArray[index].ToString();

                System.Windows.Forms.Clipboard.SetText(cellValue);
            }
        }

        private void buttonPrev_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            PrevPage();
        }

        private void buttonNext_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            NextPage();
        }

        private void MenuItemCopySQL_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            System.Windows.Forms.Clipboard.SetText(CLEAR_PATTERN.Replace(_queryBlock.QueryText, " ").Trim());
        }

        private void GridTable_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (!e.Handled)
            {
                e.Handled = true;
                var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
                eventArg.RoutedEvent = UIElement.MouseWheelEvent;
                eventArg.Source = sender;
                var parent = ((Control)sender).Parent as UIElement;
                parent.RaiseEvent(eventArg);
            }
        }

        private void GridTable_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            var binding = _block.Bindings.FirstOrDefault(p => p.Column == e.PropertyName);

            if (binding == null)
            {
                string header = e.Column.Header.ToString();
                e.Column.Header = header.Replace("_", "__"); // fucking workaround! 
            }
            else
            {
                var templateColumn = new DataGridTemplateColumn();
                templateColumn.Header = e.Column.Header.ToString().Replace("_", "__");
                e.Column = templateColumn;

                DataTemplate cellLayout = new DataTemplate();
                FrameworkElementFactory spFactory = new FrameworkElementFactory(typeof(TextBlock));
                cellLayout.VisualTree = spFactory;

                FrameworkElementFactory hyperLinkLayout = new FrameworkElementFactory(typeof(Hyperlink));
                hyperLinkLayout.AddHandler(Hyperlink.ClickEvent, new RoutedEventHandler((Object s, RoutedEventArgs ea) => Hyperlink_Click(binding, ea)));
                spFactory.AppendChild(hyperLinkLayout);

                FrameworkElementFactory innerBlockLayout = new FrameworkElementFactory(typeof(TextBlock));
                innerBlockLayout.SetBinding(TextBlock.TextProperty, new Binding(e.PropertyName));
                hyperLinkLayout.AppendChild(innerBlockLayout);

                templateColumn.CellTemplate = cellLayout;
            }
        }

        private void Hyperlink_Click(dax.Document.Binding binding, RoutedEventArgs e)
        {
            if (OnBindingClick != null)
            {
                Hyperlink hl = e.OriginalSource as Hyperlink;
                ContainerVisual container = (ContainerVisual)VisualTreeHelper.GetChild(hl.Parent as TextBlock, 0);
                String value = (container.Children[0] as TextBlock).Text;
                OnBindingClick(this, new BindingClickEventArgs(binding, value));
            }
        }

        private void TextBoxCurrentPage_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                int index;

                if (int.TryParse(textBoxCurrentPage.Text.Trim(), out index))
                {
                    index--;
                }
                else
                {
                    index = 0;
                }

                _queryBlock.PageIndex = index;
                EnableControls(false);
                _queryBlock.UpdateAsync()
                    .GetAwaiter()
                    .OnCompleted(() =>
                        {
                            RefreshPaging();
                            EnableControls(true);
                        });
            }
        }

        #endregion

        public EventHandler<BindingClickEventArgs> OnBindingClick;
    }
}
