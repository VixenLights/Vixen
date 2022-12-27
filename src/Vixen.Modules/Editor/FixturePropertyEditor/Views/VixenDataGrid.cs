using System.Reflection;
using System.Windows.Controls;

namespace VixenModules.Editor.FixturePropertyEditor.Views
{
	/// <summary>
	/// This class overrides the OnCanExecuteBeginEdit method of the standard grid
	/// </summary>
	/// <remarks>
	/// https://stackoverflow.com/questions/26057518/how-to-edit-row-in-datagrid-after-getting-some-validation-errors-in-any-one-of-t
	/// </remarks>
	public partial class VixenDataGrid : System.Windows.Controls.DataGrid
	{
		#region Constructor

		/// <summary>
		/// Construtor
		/// </summary>
		public VixenDataGrid()
		{
			// This call allows the DataGrid styles to apply to this derived grid
			this.SetResourceReference(StyleProperty, typeof(DataGrid));
		}

		#endregion

		#region Protected Methods

		/// <summary>
		/// This method overrides the 
		/// if (canExecute && HasRowValidationError) condition of the base method to allow
		/// ----entering edit mode when there is a pending validation error
		/// ---editing of other rows
		/// </summary>
		/// <param name="e"></param>
		protected override void OnCanExecuteBeginEdit(System.Windows.Input.CanExecuteRoutedEventArgs e)
		{

			bool hasCellValidationError = false;
			bool hasRowValidationError = false;
			BindingFlags bindingFlags = BindingFlags.FlattenHierarchy | BindingFlags.NonPublic | BindingFlags.Instance;
			//Current cell
			PropertyInfo cellErrorInfo = this.GetType().BaseType.GetProperty("HasCellValidationError", bindingFlags);
			//Grid level
			PropertyInfo rowErrorInfo = this.GetType().BaseType.GetProperty("HasRowValidationError", bindingFlags);

			if (cellErrorInfo != null) hasCellValidationError = (bool)cellErrorInfo.GetValue(this, null);
			if (rowErrorInfo != null) hasRowValidationError = (bool)rowErrorInfo.GetValue(this, null);

			base.OnCanExecuteBeginEdit(e);
			if (!e.CanExecute && !hasCellValidationError && hasRowValidationError)
			{
				e.CanExecute = true;
				e.Handled = true;
			}
		}

		#endregion
	}
}
