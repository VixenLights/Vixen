using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using VixenModules.Editor.FixturePropertyEditor.ViewModels;

namespace VixenModules.Editor.FixturePropertyEditor.Views
{
    /// <summary>
    /// Maintains the fixture property editor view.
    /// </summary>
    public partial class FixturePropertyEditorView : IRefreshGrid
	{
		#region Constructor
		
		/// <summary>
		/// Constructor
		/// </summary>
		public FixturePropertyEditorView()
		{
			InitializeComponent();

			// Configure the user control to display the load button
			DisplayLoadButton = true;			
		}

		#endregion
		
		#region Private Methods

		/// <summary>
		/// Scrolls the selected item into view.
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
		
		#region Public Properties
		
		/// <summary>
		/// Determines if the Load button is displayed.
		/// </summary>
		public bool DisplayLoadButton
		{
			get
			{
				return (bool) GetValue(DisplayLoadButtonProperty);
			}
			set
			{
				SetValue(DisplayLoadButtonProperty, value);
			}
		}

		public static readonly DependencyProperty DisplayLoadButtonProperty =
			DependencyProperty.Register(nameof(DisplayLoadButton), typeof(bool),
				typeof(FixturePropertyEditorView), new UIPropertyMetadata(false));

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
				FixturePropertyEditorViewModel vm = (FixturePropertyEditorViewModel)ViewModel;
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
