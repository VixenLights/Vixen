using System.ComponentModel;
using System.Runtime.Serialization;

namespace VixenModules.App.Curves
{
	[DataContract]
	public enum CurveType
	{
		[Description("Ramp Up")]
		RampUp,
		[Description("Ramp Down")]
		RampDown,
		[Description("Full 100")]
		Flat100
	}
}