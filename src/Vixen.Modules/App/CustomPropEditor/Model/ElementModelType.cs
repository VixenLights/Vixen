namespace VixenModules.App.CustomPropEditor.Model
{
	/// <summary>
	/// Specifies the role an element model has within a custom prop hierarchy.
	/// </summary>
	public enum ElementModelType
	{
		/// <summary>
		/// The element model has no special role.
		/// </summary>
		None,

		/// <summary>
		/// The element model represents the primary model element for State authoring and Preview import.
		/// </summary>
		Model,

		/// <summary>
		/// The element model represents a submodel grouping.
		/// </summary>
		SubModel,

		/// <summary>
		/// The element model represents imported or user-designated face information.
		/// </summary>
		FaceInfo,

		/// <summary>
		/// The element model represents imported or user-designated legacy State information.
		/// </summary>
		StateInfo
	}
}
