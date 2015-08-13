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

using dax.Core;
using dax.Core.Events;
using dax.Document;
using dax.Gui.Events;
using dax.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace dax.Gui
{
    /// <summary>
    /// Represents UI for single DaxDocument.
    /// </summary>
    public partial class TabDocumentControl : UserControl
    {
        private const int INPUT_ROWS_COUNT = 5;

        private readonly DaxManager _daxManager;
        private readonly INotificationView _notificationView;
        private Group _currentGroup = Group.All;

        public TabDocumentControl(DaxManager daxManager, INotificationView notificationView)
        {
            InitializeComponent();
            _daxManager = daxManager;
            _notificationView = notificationView;
            _daxManager.OnQueryReloaded += DaxManager_OnQueryReloaded;
            _daxManager.OnNewBlockAdded += DaxManager_OnNewBlockAdded;
            _daxManager.OnError += DaxManager_OnError;
            _daxManager.OnGroupsChanged += DaxManager_OnGroupsChanged;
            _daxManager.OnStateChanged += DaxManager_OnStateChanged;
            _daxManager.OnQueryFinished += DaxManager_OnQueryFinished;
            InitGrids();
        }

        public DaxManager Manager
        {
            get { return _daxManager; }
        }

        public String DocumentTitle
        {
            get
            {
                return _daxManager.FilePath;
            }
        }

        private IEnumerable<BaseInputControl> InputControls
        {
            get
            {
                return gridInputFields.Children.Cast<BaseInputControl>();
            }
        }

        private IEnumerable<TableControl> BlockControls
        {
            get
            {
                return CurrentTab.BlockControls;
            }
        }

        private IEnumerable<TabDocumentGroupControl> TabItems
        {
            get
            {
                return tabControlMain.Items
                    .Cast<TabItem>()
                    .Select(p => p.Content as TabDocumentGroupControl);
            }
        }

        private TabDocumentGroupControl CurrentTab
        {
            get
            {
                return ((TabItem)tabControlMain.SelectedItem).Content as TabDocumentGroupControl;
            }
        }

        private TabDocumentGroupControl AllQueriesTab
        {
            get
            {
                return ((TabItem)tabControlMain.Items[0]).Content as TabDocumentGroupControl;
            }
        }

        public void ReloadFile()
        {
            _daxManager.ReloadDocument();
            ReloadDocument();
        }

        public void ReloadDocument()
        {
            var oldInputs = InputControls.ToList();
            InitGrids();
            InitTabs();

            foreach (var input in _daxManager.Inputs)
            {
                BaseInputControl inputControl = BaseInputControl.Create(input);
                inputControl.OnSubmit += (s, e) => InvokeSearch();
                AddInputField(inputControl);

                var oldInput = oldInputs.FirstOrDefault(p => p.InputName == inputControl.InputName);

                if (oldInput != null)
                {
                    inputControl.IsSelected = oldInput.IsSelected;
                    inputControl.InputValue = oldInput.InputValue;
                }
            }

            RefreshState();
            RefreshConnectionStatus();
        }

        private void InitTabs()
        {
            tabControlMain.Items.Clear();

            AddTabItem(Group.All);

            if (tabControlMain.Items.Count > 0 && tabControlMain.SelectedItem == null)
            {
                tabControlMain.SelectedIndex = 0;
            }
        }

        private void AddTabItem(Group group)
        {
            var item = new TabItem() { Header = group.Name, Content = new TabDocumentGroupControl(group) };
            tabControlMain.Items.Add(item);
        }

        public void InvokeSearch()
        {
            if (_daxManager.CanSearch)
            {
                InputControls.ToList().ForEach(p => p.IsHighlighted = false);

                var inputs = InputControls.Where(p => p.IsSelected)
                                .Where(p => p.Context.AllowBlank || !p.IsBlank)
                                .ToList();

                inputs.ForEach(p => p.IsHighlighted = true);

                var deselectedGroups = BlockControls
                    .Where(p => !p.IsSelected)
                    .Select(p => p.Block)
                    .ToList();

                CurrentTab.DeselectedBlocks.AddRange(deselectedGroups);

                if (BlockControls.All(p => !p.IsSelected))
                {
                    CurrentTab.DeselectedBlocks.Clear();
                }

                _currentGroup = CurrentTab.Group;
                List<Block> deselectedBlocks = CurrentTab.DeselectedBlocks;

                var inputValues = inputs.ToDictionary(p => p.InputName, p => p.InputValue);
                _daxManager.SearchAsync(inputValues, b => BlockFilter(b, _currentGroup, deselectedBlocks));
            }
        }

        private void InvokeCancel()
        {
            if (_daxManager.CanCancel)
            {
                _daxManager.Cancel();
            }
        }

        private bool BlockFilter(Block target, Group currentGroup, List<Block> deselectedBlocks)
        {
            return (deselectedBlocks.Count == 0 || deselectedBlocks.All(p => p.Order != target.Order))
                && (currentGroup.IsAll || target.HasGroup(currentGroup));
        }

        private void RefreshConnectionStatus()
        {
            buttonReconnect.Content = _daxManager.IsConnected ? "Reconnect" : "Connect";
        }

        private void RefreshState()
        {
            switch (_daxManager.CurrentState)
            {
                case OperationState.Ready:
                    buttonSearch.Content = "Search";
                    buttonSearch.IsEnabled = true;
                    buttonReconnect.IsEnabled = true;
                    buttonReload.IsEnabled = true;
                    break;
                case OperationState.Searching:
                    buttonSearch.Content = "Cancel";
                    buttonSearch.IsEnabled = true;
                    buttonReconnect.IsEnabled = false;
                    buttonReload.IsEnabled = false;
                    _notificationView.SetStatus("Loading...");
                    break;
                default:
                    throw new InvalidOperationException("Unsupported state: " + _daxManager.CurrentState);
            }
        }

        private void AddInputField(BaseInputControl input)
        {
            gridInputFields.Children.Add(input);
            int rowCount = (gridInputFields.Children.Count + (INPUT_ROWS_COUNT - 1)) / INPUT_ROWS_COUNT;

            if (gridInputFields.RowDefinitions.Count < rowCount)
            {
                gridInputFields.RowDefinitions.Add(new RowDefinition());
            }

            Grid.SetRow(input, gridInputFields.RowDefinitions.Count - 1);
            Grid.SetColumn(input, (gridInputFields.Children.Count - 1) % INPUT_ROWS_COUNT);
        }

        private void InitGrids()
        {
            gridInputFields.Children.Clear();
            gridInputFields.RowDefinitions.Clear();
            gridInputFields.ColumnDefinitions.Clear();
            InitContentGrid();


            for (int i = 0; i < INPUT_ROWS_COUNT; i++)
            {
                gridInputFields.ColumnDefinitions.Add(new ColumnDefinition());
            }
        }

        private void InitContentGrid()
        {
            if (tabControlMain.Items.Count > 0)
            {
                if (_currentGroup.IsAll)
                {
                    AllQueriesTab.InitContentGrid();
                    return;
                }

                var tab = TabItems.FirstOrDefault(p => p.Group.Equals(_currentGroup));

                if (tab != null)
                {
                    tab.InitContentGrid();
                }
            }
        }

        #region Event Handlers

        private void ButtonSearchOrCancel_Click(object sender, RoutedEventArgs e)
        {
            if (_daxManager.CanSearch)
            {
                InvokeSearch();
            }
            else if (_daxManager.CanCancel)
            {
                InvokeCancel();
            }

            RefreshConnectionStatus();
        }

        private void DaxManager_OnNewBlockAdded(object sender, NewBlockAddedEventArgs e)
        {
            if (_currentGroup.IsAll)
            {
                AllQueriesTab.AddBlock(e.Block, e.QueryBlock, _notificationView, BindingHandler);
            }

            CheckTabCreated();

            var tab = TabItems.FirstOrDefault(p => p.Group.Equals(_currentGroup));

            if (tab != null)
            {
                tab.AddBlock(e.Block, e.QueryBlock, _notificationView, BindingHandler);
            }

            RefreshConnectionStatus();
        }

        private void CheckTabCreated()
        {
            if (TabItems.Count() <= 1)
            {
                foreach (Group group in _daxManager.Groups)
                {
                    AddTabItem(group);
                }
            }
        }

        private void BindingHandler(BindingClickEventArgs e)
        {
            var inputValue = InputControls.FirstOrDefault(p => String.Compare(p.InputName, e.Binding.Field, true) == 0);

            if (inputValue != null)
            {
                inputValue.InputValue = e.Value;
                inputValue.IsSelected = true;
                InvokeSearch();
            }
        }

        private void DaxManager_OnQueryReloaded(object sender, QueryReloadedEventArgs e)
        {
            InitContentGrid();
        }

        private void DaxManager_OnError(object sender, ErrorEventArgs e)
        {
            if (String.IsNullOrEmpty(e.Query))
            {
                _notificationView.ShowError(e.Message);
            }
            else
            {
                String message = String.Format("Failed to execute query: {1}{0}{0}{3}{0}{2}{0}{3}",
                    Environment.NewLine, e.Message, e.Query, "--------");

                _notificationView.ShowError(message);
            }
        }

        private void DaxManager_OnGroupsChanged(object sender, EventArgs e)
        {
            InitTabs();
        }

        private void ButtonReconnect_Click(object sender, RoutedEventArgs e)
        {
            var task = _daxManager.Reconnect();

            task.GetAwaiter().OnCompleted(RefreshConnectionStatus);
        }

        private void ButtonReload_Click(object sender, RoutedEventArgs e)
        {
            ReloadFile();
        }

        private void SelectAll_Cliked(object sender, RoutedEventArgs e)
        {
            InputControls.ToList().ForEach(p => p.IsSelected = true);
        }

        private void SelectNone_Cliked(object sender, RoutedEventArgs e)
        {
            InputControls.ToList().ForEach(p => p.IsSelected = false);
        }

        private void ResetFilter_Click(object sender, RoutedEventArgs e)
        {
            InputControls.ToList().ForEach(p => p.Reset());
            InvokeSearch();
        }

        private void SelectAllBlocks_Clicked(object sender, RoutedEventArgs e)
        {
            BlockControls.ToList().ForEach(p => p.IsSelected = true);
        }

        private void SelectNoneBlocks_Clicked(object sender, RoutedEventArgs e)
        {
            BlockControls.ToList().ForEach(p => p.IsSelected = false);
        }

        private void ResetBlocks_Clicked(object sender, RoutedEventArgs e)
        {
            BlockControls.ToList().ForEach(p => p.IsSelected = false);
            InvokeSearch();
        }

        private void DaxManager_OnStateChanged(object sender, EventArgs e)
        {
            RefreshState();
        }

        private void DaxManager_OnQueryFinished(object sender, QueryFinishedEventArgs e)
        {
            if (e.Canceled)
            {
                _notificationView.SetStatus("Operation canceled by user");
            }
            else
            {
                _notificationView.SetStatus(String.Format("Queries ({0}) executed in {1}",
                            e.QueryCount, TimeUtils.PrettifyTime(e.ElapsedTime)));
            }
        }

        #endregion
    }
}
