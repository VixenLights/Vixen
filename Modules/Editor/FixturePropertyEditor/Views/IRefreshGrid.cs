namespace VixenModules.Editor.FixturePropertyEditor.Views
{
	/// <summary>
	/// Interface to allow view model to refresh grid(s) contained in the view.
	/// </summary>
	/// <remarks>
	/// This interface is necessary because the WPF Datagrid seems to hold
	/// onto invalid rows after they have been deleted from the view model.
	/// Without refreshing the grid it is left in a read-only state.
	/// </remarks>
	internal interface IRefreshGrid
	{
		/// <summary>
		/// Refreshes the grid(s) in the view.
		/// </summary>
		void Refresh();
	}
}
