using System.ComponentModel;

namespace VixenModules.Effect.Circles
{
	public enum CircleType
	{
		[Description("Bounce")]
		Bounce,
		[Description("Wrap")]
		Wrap,
		[Description("Radial")]
		Radial,
		[Description("Radial3D")]
		Radial3D,
		[Description("Circles")]
		Circles
	}
}