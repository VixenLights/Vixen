using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;
using VixenModules.Editor.FixturePropertyEditor.ViewModels;

namespace VixenModules.Editor.FixturePropertyEditor.Views
{
	/// <summary>
	/// Maintains a color wheel view.
	/// </summary>
	public partial class ColorWheelView : DataGridView, IRefreshGrid
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		public ColorWheelView()
		{
			InitializeComponent();
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Scrolls the selected color item into view.
		/// </summary>
		/// <param name="sender">Event sender</param>
		/// <param name="e">Event arguments</param>
		private void SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			// Get a reference to the data grid
			DataGrid obj = sender as DataGrid;

			// If there is a selected item then...
			if (obj != null && obj.SelectedItem != null)
			{
				// Retrieve the view model from Catel base class
				ColorWheelViewModel vm = (ColorWheelViewModel)ViewModel;

				// If a new row being added then...
				if (vm.AddItemInProgress)
				{
					// Give the grid the focus
					obj.Focus();
				}

				// Scroll the selected into view
				obj.ScrollIntoView(obj.SelectedItem);

				// If a new row being added then...
				if (vm.AddItemInProgress)
				{
					// Put the first cell into edit
					DataGridCellInfo cellInfo = new DataGridCellInfo(obj.SelectedItem, grid.Columns[0]);
					obj.CurrentCell = cellInfo;
					obj.BeginEdit();
				}
			}
		}

		#endregion
		
		#region IRefrsehGrid

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public void Refresh()
		{
			try
			{
				// Cancel any pending edits
				ColorWheelViewModel vm = (ColorWheelViewModel)ViewModel;
				IEditableCollectionView collectionView = (IEditableCollectionView)CollectionViewSource.GetDefaultView(vm.Items);
				collectionView.CancelEdit();
			}
			catch (Exception)
			{
				// Testing revealed deleting 2nd or 3rd incomplete row seemed to trigger an exception
			}

			// Refresh the items in the DataGrid.
			// This method exists because deleting invalid rows in grid was basically leaving the grid in
			// a read-only state because it seemed to hang onto the invalid row.
			grid.Items.Refresh();
		}

		#endregion
	}
}
