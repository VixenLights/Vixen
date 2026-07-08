namespace VixenModules.Preview.VixenPreview
{
	/// <summary>
	/// Provides visibility rules for the Preview Setup visual canvas.
	/// </summary>
	public static class PreviewCanvasVisibility
	{
		/// <summary>
		/// Determines whether a display item should be visible on the Preview Setup canvas.
		/// </summary>
		/// <param name="hideLockedDisplayItems"><see langword="true" /> to hide locked display items; otherwise, <see langword="false" />.</param>
		/// <param name="displayItemLocked"><see langword="true" /> when the display item is locked; otherwise, <see langword="false" />.</param>
		/// <returns><see langword="true" /> if the display item should be visible; otherwise, <see langword="false" />.</returns>
		public static bool IsDisplayItemVisible(bool hideLockedDisplayItems, bool displayItemLocked)
		{
			return !hideLockedDisplayItems || !displayItemLocked;
		}

		/// <summary>
		/// Determines whether the hide/show locked display items command should be enabled.
		/// </summary>
		/// <param name="hideLockedDisplayItems"><see langword="true" /> when locked display items are currently hidden; otherwise, <see langword="false" />.</param>
		/// <param name="anyLockedDisplayItems"><see langword="true" /> when any display item is locked; otherwise, <see langword="false" />.</param>
		/// <returns><see langword="true" /> if the command should be enabled; otherwise, <see langword="false" />.</returns>
		public static bool IsHideLockedCommandEnabled(bool hideLockedDisplayItems, bool anyLockedDisplayItems)
		{
			return hideLockedDisplayItems || anyLockedDisplayItems;
		}

		/// <summary>
		/// Determines whether a selected display item should remain selected when hide mode changes.
		/// </summary>
		/// <param name="hideLockedDisplayItems"><see langword="true" /> to hide locked display items; otherwise, <see langword="false" />.</param>
		/// <param name="displayItemLocked"><see langword="true" /> when the display item is locked; otherwise, <see langword="false" />.</param>
		/// <returns><see langword="true" /> if the display item should remain selected; otherwise, <see langword="false" />.</returns>
		public static bool ShouldRetainSelection(bool hideLockedDisplayItems, bool displayItemLocked)
		{
			return IsDisplayItemVisible(hideLockedDisplayItems, displayItemLocked);
		}
	}
}
