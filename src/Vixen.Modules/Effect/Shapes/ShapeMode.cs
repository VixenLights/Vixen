using System.ComponentModel;

namespace VixenModules.Effect.Shapes
{
	public enum ShapeMode
	{
		[Description("None")]
		None,
		[Description("Create Shapes from Mark Collection")]
		CreateShapesMarkCollection,
		[Description("Remove Shapes from Mark Collection")]
		RemoveShapesMarkCollection
	}
}
