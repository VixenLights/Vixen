using System.Runtime.Serialization;

namespace VixenModules.App.Curves
{
	[DataContract]
	public enum CurveType
	{
		RampUp,
		RampDown,
		Flat100
	}
}