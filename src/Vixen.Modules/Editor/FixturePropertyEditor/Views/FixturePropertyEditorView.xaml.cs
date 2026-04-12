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
    public partial class FixturePropertyEditorView : DataGridView, IRefreshGrid
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
