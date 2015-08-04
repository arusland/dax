using dax.Core;
using dax.Core.Events;
using dax.Document;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using dax.Utils;

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
        private OperationState _currentState;

        public TabDocumentControl(DaxManager daxManager, INotificationView notificationView)
        {
            InitializeComponent();
            _daxManager = daxManager;
            _notificationView = notificationView;
            _daxManager.OnQueryReloaded += DaxManager_OnQueryReloaded;
            _daxManager.OnNewBlockAdded += DaxManager_OnNewBlockAdded;
            _daxManager.OnError += DaxManager_OnError;
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

        private OperationState CurrentState
        {
            set
            {
                _currentState = value;
                RefreshState();
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
                return gridBlocks.Children.Cast<TableControl>();
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

        public void InvokeSearch()
        {
            if (_currentState == OperationState.Ready)
            {
                InputControls.ToList().ForEach(p => p.IsHighlighted = false);

                var inputs = InputControls.Where(p => p.IsSelected)
                                .Where(p => p.Context.AllowBlank || !p.IsBlank)
                                .ToList();

                inputs.ForEach(p => p.IsHighlighted = true);

                var selectedQueries = BlockControls
                    .Where(p => p.IsSelected)
                    .Select(p => p.Block)
                    .ToList();

                _notificationView.SetStatus("Loading...");
                buttonSearch.IsEnabled = false;
                var inputValues = inputs.ToDictionary(p => p.InputName, p => p.InputValue);
                var watcher = Stopwatch.StartNew();
                var task = _daxManager.ReloadAsync(inputValues, b => BlockFilter(b, selectedQueries));
                task.GetAwaiter().OnCompleted(() =>
                {
                    if (_currentState == OperationState.Canceling)
                    {
                        _notificationView.SetStatus("Operation canceled by user");
                    }
                    else
                    {
                        watcher.Stop();
                        _notificationView.SetStatus(String.Format("Queries executed in {0}", TimeUtils.PrettifyTime(watcher.ElapsedMilliseconds)));
                    }

                    CurrentState = OperationState.Ready;
                });
                CurrentState = OperationState.Searching;
            }
        }

        private bool BlockFilter(Block target, List<Block> selectedBlocks)
        {
            return selectedBlocks.Count == 0 || selectedBlocks.Any(p => p.Order == target.Order);
        }

        public void InvokeCancelSearch()
        {
            if (_currentState == OperationState.Searching)
            {
                _notificationView.SetStatus("Canceling...");
                CurrentState = OperationState.Canceling;
            }
        }

        private void RefreshConnectionStatus()
        {
            buttonReconnect.Content = _daxManager.IsConnected ? "Reconnect" : "Connect";
        }

        private void RefreshState()
        {
            switch (_currentState)
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
                    break;
                case OperationState.Canceling:
                    buttonSearch.Content = "Canceling...";
                    buttonSearch.IsEnabled = false;
                    buttonReconnect.IsEnabled = false;
                    buttonReload.IsEnabled = false;
                    break;
                default:
                    throw new InvalidOperationException("Unsupported state: " + _currentState);
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
            gridBlocks.Children.Clear();
            gridBlocks.RowDefinitions.Clear();
        }

        #region Event Handlers

        private void buttonSearch_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (_currentState == OperationState.Ready)
            {
                InvokeSearch();
            }
            else if (_currentState == OperationState.Searching)
            {
                InvokeCancelSearch();
            }

            RefreshConnectionStatus();
        }

        private void DaxManager_OnNewBlockAdded(object sender, NewBlockAddedEventArgs e)
        {
            var tableItem = new TableControl(e.Block, e.QueryBlock, _notificationView);
            tableItem.OnBindingClick += OnBinding_Click;
            gridBlocks.Children.Add(tableItem);

            while (e.Block.Order >= gridBlocks.RowDefinitions.Count)
            {
                gridBlocks.RowDefinitions.Add(new RowDefinition());
            }

            Grid.SetRow(tableItem, e.Block.Order);

            e.Canceled = _currentState == OperationState.Canceling;
            RefreshConnectionStatus();
        }

        private void OnBinding_Click(object sender, Events.BindingClickEventArgs e)
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

        #endregion
    }
}
