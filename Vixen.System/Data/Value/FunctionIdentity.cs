using System.ComponentModel;
using Vixen.TypeConverters;

namespace Vixen.Data.Value
{
	/// <summary>
	/// Defines specific functions.  This enum allows the Vixen Preview and filtering to support
	/// specific functions.
	/// </summary>
	[TypeConverter(typeof(EnumDescriptionTypeConverter))]
	public enum FunctionIdentity
	{
		[Description("Pan")]
		Pan,

		[Description("Tilt")]
		Tilt,

		[Description("Zoom")]
		Zoom,

		[Description("Dim")]
		Dim,

		[Description("Shutter")]
		Shutter,

		[Description(" ")]
		Custom,
	}
}
