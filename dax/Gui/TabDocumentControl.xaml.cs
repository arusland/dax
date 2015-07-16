using dax.Core;
using dax.Core.Events;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public IEnumerable<InputControl> InputControls
        {
            get
            {
                return gridInputFields.Children.Cast<InputControl>();
            }
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

        public void ReloadFile()
        {
            _daxManager.ReloadDocument();
            ReloadDocument();
        }

        public void ReloadDocument()
        {
            InitGrids();

            foreach (var input in _daxManager.Inputs)
            {
                var inputControl = new InputControl(input);
                inputControl.OnSubmit += (s, e) => InvokeSearch();
                AddInputField(inputControl);
            }

            RefreshState();
            RefreshConnectionStatus();
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
                    break;
                case OperationState.Searching:
                    buttonSearch.Content = "Cancel";
                    buttonSearch.IsEnabled = true;
                    break;
                case OperationState.Canceling:
                    buttonSearch.Content = "Canceling...";
                    buttonSearch.IsEnabled = false;
                    break;
                default:
                    throw new InvalidOperationException("Unsupported state: " + _currentState);
            }
        }

        private void AddInputField(InputControl input)
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
                _notificationView.SetStatus("Canceling...");
                CurrentState = OperationState.Canceling;
            }

            RefreshConnectionStatus();
        }

        private void InvokeSearch()
        {
            if (_currentState == OperationState.Ready)
            {
                var inputs = InputControls.Where(p => p.IsSelected)
                                .Where(p => p.Context.AllowBlank || !String.IsNullOrEmpty(p.InputValue))
                                .ToList();

                _notificationView.SetStatus("Loading...");
                buttonSearch.IsEnabled = false;
                var inputValues = inputs.ToDictionary(p => p.InputName, p => p.InputValue);
                var task = _daxManager.ReloadAsync(inputValues);
                task.GetAwaiter().OnCompleted(() =>
                {
                    if (_currentState == OperationState.Canceling)
                    {
                        _notificationView.SetStatus("Operation canceled by user");
                    }
                    else
                    {
                        _notificationView.SetStatus(String.Empty);
                    }

                    CurrentState = OperationState.Ready;
                });
                CurrentState = OperationState.Searching;
            }
        }
        private void DaxManager_OnNewBlockAdded(object sender, NewBlockAddedEventArgs e)
        {
            var tableitem = new TableControl(e.Block, e.QueryBlock, _notificationView);
            gridBlocks.RowDefinitions.Add(new RowDefinition());
            gridBlocks.Children.Add(tableitem);
            Grid.SetRow(tableitem, gridBlocks.RowDefinitions.Count - 1);

            e.Canceled = _currentState == OperationState.Canceling;
            RefreshConnectionStatus();
        }

        private void DaxManager_OnQueryReloaded(object sender, QueryReloadedEventArgs e)
        {
            InitContentGrid();
        }

        private void DaxManager_OnError(object sender, ErrorEventArgs e)
        {
            _notificationView.ShowError(e.Message);
        }

        private void ButtonReconnect_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var task = _daxManager.Reconnect();

            task.GetAwaiter().OnCompleted(RefreshConnectionStatus);
        }

        private void ButtonReload_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ReloadFile();
        }

        #endregion
    }
}
