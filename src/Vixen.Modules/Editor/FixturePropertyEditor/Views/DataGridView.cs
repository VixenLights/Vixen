using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace VixenModules.Editor.FixturePropertyEditor.Views
{
	/// <summary>
	/// Shared base class for UserControl views that contain a data grid.
	/// </summary>
	/// <remarks>This class provides some event handlers that makes it easier to put the grid into edit.</remarks>
	public class DataGridView : Catel.Windows.Controls.UserControl
	{
		#region Protected EditCellInOneClick Methods

		/// <summary>
		/// Datagrid event when a cell receives focus.
		/// </summary>
		/// <param name="sender">Event sender</param>
		/// <param name="e">Event arguments</param>
		/// <remarks>This solution was found here:
		/// https://stackoverflow.com/questions/3426765/single-click-edit-in-wpf-datagrid
		/// </remarks>
		protected void DataGrid_CellGotFocus(object sender, RoutedEventArgs e)
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
		protected T GetFirstChildByType<T>(DependencyObject prop) where T : DependencyObject
		{
			for (int i = 0; i < VisualTreeHelper.GetChildrenCount(prop); i++)
			{
				DependencyObject child = VisualTreeHelper.GetChild(prop, i);

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
	}
}
