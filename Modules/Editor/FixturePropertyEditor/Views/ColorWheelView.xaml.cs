using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using VixenModules.Editor.FixturePropertyEditor.ViewModels;

namespace VixenModules.Editor.FixturePropertyEditor.Views
{
	/// <summary>
	/// Maintains a color wheel view.
	/// </summary>
	public partial class ColorWheelView : IRefreshGrid
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
				// Scroll the selected into view
				obj.ScrollIntoView(obj.SelectedItem);
			}
		}

		#endregion

		#region Private EditCellInOneClick Methods

		/// <summary>
		/// Datagrid event when a cell receives focus.
		/// </summary>
		/// <param name="sender">Event sender</param>
		/// <param name="e">Event arguments</param>
		/// <remarks>This solution was found here:
		/// https://stackoverflow.com/questions/3426765/single-click-edit-in-wpf-datagrid
		/// </remarks>
		private void DataGrid_CellGotFocus(object sender, RoutedEventArgs e)
		{
			// Lookup for the source to be DataGridCell
			if (e.OriginalSource.GetType() == typeof(DataGridCell))
			{
				// Starts the Edit on the row;
				DataGrid grd = (DataGrid)sender;
				grd.BeginEdit(e);

				Control control = GetFirstChildByType<Control>(e.OriginalSource as DataGridCell);
				if (control != null)
				{
					control.Focus();
				}
			}
		}

		/// <summary>
		/// Refer to https://stackoverflow.com/questions/3426765/single-click-edit-in-wpf-datagrid for more information.
		/// </summary>		
		private T GetFirstChildByType<T>(DependencyObject prop) where T : DependencyObject
		{
			for (int i = 0; i < VisualTreeHelper.GetChildrenCount(prop); i++)
			{
				DependencyObject child = VisualTreeHelper.GetChild((prop), i) as DependencyObject;
				if (child == null)
					continue;

				T castedProp = child as T;
				if (castedProp != null)
					return castedProp;

				castedProp = GetFirstChildByType<T>(child);

				if (castedProp != null)
					return castedProp;
			}

			return null;
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
			catch (Exception e)
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
