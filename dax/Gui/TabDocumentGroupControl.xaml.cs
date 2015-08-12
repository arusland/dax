/*
 * Copyright 2015 the original author or authors.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

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
