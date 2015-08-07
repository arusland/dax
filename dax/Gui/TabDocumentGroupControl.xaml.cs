using dax.Db;
using dax.Document;
using dax.Gui.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace dax.Gui
{
    public partial class TabDocumentGroupControl : UserControl
    {
        public TabDocumentGroupControl(Group group)
        {
            InitializeComponent();
            Group = group;
            DeselectedBlocks = new List<Block>();
        }

        public Group Group
        {
            get;
            private set;
        }

        public IEnumerable<TableControl> BlockControls
        {
            get
            {
                return gridBlocks.Children.Cast<TableControl>();
            }
        }

        public List<Block> DeselectedBlocks
        {
            get;
            private set;
        }

        public void InitContentGrid()
        {
            gridBlocks.Children.Clear();
            gridBlocks.RowDefinitions.Clear();
        }

        public void AddBlock(dax.Document.Block block, IQueryBlock queryBlock, INotificationView notificationView, Action<BindingClickEventArgs> bindingHandler)
        {
            var tableItem = new TableControl(block, queryBlock, notificationView);
            tableItem.OnBindingClick += (s, e) => bindingHandler(e);
            gridBlocks.Children.Add(tableItem);

            while (block.Order >= gridBlocks.RowDefinitions.Count)
            {
                gridBlocks.RowDefinitions.Add(new RowDefinition());
            }

            Grid.SetRow(tableItem, block.Order);
        }
    }
}
