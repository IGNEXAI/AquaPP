using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;

namespace AquaPP.Views.Pages;

public partial class DataEntryView : UserControl
{
    public DataEntryView()
    {
        InitializeComponent();
    }

    private void DataGridPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        // We only care about left-clicks
        if (!e.GetCurrentPoint(sender as Visual).Properties.IsLeftButtonPressed)
        {
            return;
        }

        // Find the cell that was clicked.
        // We get the original source of the event and walk up the visual tree to find the DataGridCell.
        var cell = (e.Source as Visual)?.GetSelfAndVisualAncestors()
            .OfType<DataGridCell>()
            .FirstOrDefault();

        if (cell == null)
        {
            return;
        }

        // Get the DataGrid itself from the 'sender' argument
        if (sender is not DataGrid grid)
        {
            return;
        }

        // Prevent editing if the grid is read-only or the cell is already being edited
        if (grid.IsReadOnly || cell.IsFocused)
        {
            return;
        }

        // Find the parent row
        var row = cell.GetVisualParent<DataGridRow>();
        if (row == null)
        {
            return;
        }

        // Set the grid's current item and column to the one that was clicked
        grid.SelectedItem = row.DataContext;

        // Begin the edit
        if (grid.BeginEdit())
        {
            e.Handled = true;
        }
    }
}