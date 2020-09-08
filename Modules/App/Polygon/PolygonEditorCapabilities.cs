namespace VixenModules.App.Polygon
{
	/// <summary>
	/// Configures the capabilities of the Polygon Editor.
	/// </summary>
	public class PolygonEditorCapabilities
	{
		#region Public Properties
		
		/// <summary>
		/// True when deleting polygon point should be enabled.
		/// </summary>
		public bool DeletePoints { get; set; }

		/// <summary>
		/// True when adding polygon/lines/ellipses should be enabled.
		/// </summary>
		public bool AddPolygons { get; set; }
		
		/// <summary>
		/// True when adding polygon points should be enabled.
		/// </summary>
		public bool AddPoint { get; set; }

		/// <summary>
		/// True when pasting shapes should be enabled.
		/// </summary>
		public bool PastePolygons { get; set; }

		/// <summary>
		/// True when copying polygons should be enabled.
		/// </summary>
		public bool CopyPolygons { get; set; }

		/// <summary>
		/// True when pasting shapes should be enabled.
		/// </summary>
		public bool CutPolygons { get; set; }

		/// <summary>
		/// True when deleting polygon/lines should be enabled.
		/// </summary>
		public bool DeletePolygons { get; set; }

		/// <summary>
		/// True when the editor should default to selection mode.
		/// </summary>
		public bool DefaultToSelect { get; set; }

		/// <summary>
		/// True when the toggle wipe start side should be enabled.
		/// </summary>
		public bool ToggleStartSide { get; set; }

		/// <summary>
		/// True when the toggle start point should be enabled.
		/// </summary>
		public bool ToggleStartPoint { get; set; }

		/// <summary>
		/// True when the polygon start side should be colored.
		/// </summary>
		public bool ShowStartSide { get; set; }

		/// <summary>
		/// True when the time bar should be visible.
		/// </summary>
		public bool ShowTimeBar { get; set; }

		/// <summary>
		/// True when the user is allowed to draw more than one shape.
		/// </summary>
		public bool AllowMultipleShapes { get; set; }

		#endregion
	}
}
