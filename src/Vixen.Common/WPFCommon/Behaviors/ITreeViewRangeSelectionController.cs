namespace Common.WPFCommon.Behaviors
{
	/// <summary>
	/// Defines selection operations used by <see cref="TreeViewRangeSelectionBehavior" />.
	/// </summary>
	public interface ITreeViewRangeSelectionController
	{
		/// <summary>
		/// Selects a single item and clears the previous selection.
		/// </summary>
		/// <param name="item">The item to select.</param>
		void SelectSingle(object item);

		/// <summary>
		/// Adds or removes one item from the current selection.
		/// </summary>
		/// <param name="item">The item whose selection state is toggled.</param>
		void ToggleSelection(object item);

		/// <summary>
		/// Selects a range between the current anchor item and the specified item.
		/// </summary>
		/// <param name="item">The item that completes the range.</param>
		void SelectRange(object item);

		/// <summary>
		/// Toggles the checked state for the currently selected items.
		/// </summary>
		void ToggleCheckedStateForSelectedItems();
	}
}
