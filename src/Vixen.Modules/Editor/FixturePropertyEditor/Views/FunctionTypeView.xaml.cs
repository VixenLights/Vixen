using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using VixenModules.Editor.FixturePropertyEditor.ViewModels;

namespace VixenModules.Editor.FixturePropertyEditor.Views
{
    /// <summary>
    /// Maintains a fixture function editor view.
    /// </summary>
    public partial class FunctionTypeView : IRefreshGrid
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>	
		public FunctionTypeView()
		{
			InitializeComponent();

			Width = 1100;
			Height = 500;					
		}

		#endregion
		
		#region Private Methods
								
		/// <summary>
		/// Event handler for when selected function change is attempted.
		/// </summary>
		/// <param name="sender">Event sender</param>
		/// <param name="e">Event arguments</param>
		private void SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			// Get a reference to the FunctionType view model
			FunctionTypeViewModel vm = (FunctionTypeViewModel)ViewModel;

			// Get the function data grid from the sender event argument
			DataGrid obj = sender as DataGrid;

			// If the selected item is null or
			// the view model has been released then...
			if (vm == null ||				
				vm.SelectedItem == null ||
				obj.SelectedItem == null)
            {
				// Ignore the event
				return;
            }
			
			// By default the DataGrid selects the first row 			
			if (vm.InitialSelectedFunction != null && 
				vm.InitialSelectedFunction != obj.SelectedItem)				 
            {
				// Ignore this selection
				return;
            }
			
			// For some reason this method gets called when closing the window
			// and the view model has been released
			if (vm != null)
			{																		
				// If the selection is allowed to change then...
				if (vm.AllowSelectionToChange())
				{					
					// If there is a selected item then...
					if (obj != null && 
						obj.SelectedItem != null &&
						vm.SelectedItem != null)
					{
						// Select the specified function and display the details
						vm.SelectFunctionItem((FunctionItemViewModel)obj.SelectedItem);

						// If a new row being added then...
						if (vm.AddItemInProgress)
						{
							// Need to defer this logic otherwise the grid loses focus
							Dispatcher.InvokeAsync(() =>
							{
								// Give the grid focus
								obj.Focus();

								// Scroll the selected into view
								obj.ScrollIntoView(obj.SelectedItem);

								// Put the first cell into edit
								DataGridCellInfo cellInfo = new DataGridCellInfo(obj.SelectedItem, obj.Columns[0]);
								obj.CurrentCell = cellInfo;
								obj.BeginEdit();
							});

						}
						else
						{
							// Scroll the selected into view
							obj.ScrollIntoView(obj.SelectedItem);
						}
					}
				}
				// Not allowing the user to change rows because of incomplete data
				else
				{
					// Restore the previous function putting focus in the name column
					RestorePreviouslySelectedFunction();
				}
			}
		}

		/// <summary>
		/// Restore the previously selected function.
		/// </summary>
		private void RestorePreviouslySelectedFunction()
        {
			// Doing this on the dispatcher so that the selection event can complete
			Dispatcher.InvokeAsync(() =>
			{
				// Get a reference to the FunctionType view model
				FunctionTypeViewModel vm = (FunctionTypeViewModel)ViewModel;

				// Reselect the prevous item
				functionGrid.SelectedItem = vm.PreviouslySelectedItem;

				// Get the data grid row of the previously selected row
				DataGridRow row = functionGrid.ItemContainerGenerator.ContainerFromIndex(functionGrid.SelectedIndex) as DataGridRow;

				// Get the Name column
				DataGridCell cell = GetCell(functionGrid, row, 0);

				// Force focus to the Name column
				cell.Focus();
			});
		}
        				
		#endregion

		#region Microsoft Private Methods

		/// <summary>
		/// Finds a visual child.
		/// </summary>		
		/// <remarks>
		/// This code is from:
		/// https://social.technet.microsoft.com/wiki/contents/articles/21202.wpf-programmatically-selecting-and-focusing-a-row-or-cell-in-a-datagrid.aspx?Sort=MostUseful&PageIndex=1
		/// </remarks>
		private static T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
		{
			for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
			{
				DependencyObject child = VisualTreeHelper.GetChild(obj, i);
				if (child != null && child is T)
					return (T)child;
				else
				{
					T childOfChild = FindVisualChild<T>(child);
					if (childOfChild != null)
						return childOfChild;
				}
			}
			return null;
		}

		/// <summary>
		/// Retrieves the specified data grid cell.
		/// </summary>
		/// <param name="dataGrid">Datagrid to process</param>
		/// <param name="rowContainer">Row to retrieve the cell from</param>
		/// <param name="column">Column to retrive the cell from</param>
		/// <returns></returns>
		private static DataGridCell GetCell(DataGrid dataGrid, DataGridRow rowContainer, int column)
		{
			if (rowContainer != null)
			{
				DataGridCellsPresenter presenter = FindVisualChild<DataGridCellsPresenter>(rowContainer);
				if (presenter == null)
				{
					/* if the row has been virtualized away, call its ApplyTemplate() method 
					 * to build its visual tree in order for the DataGridCellsPresenter
					 * and the DataGridCells to be created */
					rowContainer.ApplyTemplate();
					presenter = FindVisualChild<DataGridCellsPresenter>(rowContainer);
				}
				if (presenter != null)
				{
					DataGridCell cell = presenter.ItemContainerGenerator.ContainerFromIndex(column) as DataGridCell;
					if (cell == null)
					{
						/* bring the column into view
						 * in case it has been virtualized away */
						dataGrid.ScrollIntoView(rowContainer, dataGrid.Columns[column]);
						cell = presenter.ItemContainerGenerator.ContainerFromIndex(column) as DataGridCell;
					}
					return cell;
				}
			}
			return null;
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
				FunctionTypeViewModel vm = (FunctionTypeViewModel)ViewModel;
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
			functionGrid.Items.Refresh();
		}

		#endregion
	}
}
