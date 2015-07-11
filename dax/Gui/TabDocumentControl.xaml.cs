using dax.Core;
using dax.Core.Events;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Linq;

namespace dax.Gui
{
    /// <summary>
    /// Represents UI for single DaxDocument.
    /// </summary>
    public partial class TabDocumentControl : UserControl
    {
        private const int INPUT_ROWS_COUNT = 5;

        private readonly DaxManager _daxManager;

        public TabDocumentControl(DaxManager daxManager)
        {
            InitializeComponent();
            _daxManager = daxManager;
            _daxManager.OnQueryReloaded += DaxManager_OnQueryReloaded;
            _daxManager.OnNewBlockAdded += DaxManager_OnNewBlockAdded;
            InitGrids();
        }

        public DaxManager Manager
        {
            get { return _daxManager; }
        }

        public IEnumerable<InputControl> SelectedInputControls
        {
            get
            {
                return gridInputFields.Children.Cast<InputControl>();
            }
        }

        public void ReloadDocument()
        {
            InitGrids();

            foreach (var input in _daxManager.Inputs)
            {
                var inputControl = new InputControl(input);
                AddInputField(inputControl);
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
            var inputs = SelectedInputControls.Where(p => p.IsSelected).ToList();

            if (inputs.Count > 0)
            {
                var inputValues = inputs.ToDictionary(p => p.InputName, p => p.InputValue);
                _daxManager.Reload(inputValues);
            }
        }

        private void DaxManager_OnNewBlockAdded(object sender, NewBlockAddedEventArgs e)
        {
            var tableitem = new TableControl(e.Block, e.QueryBlock);
            gridBlocks.Children.Add(tableitem);
        }

        private void DaxManager_OnQueryReloaded(object sender, QueryReloadedEventArgs e)
        {
            InitContentGrid();
        }

        #endregion
    }
}
