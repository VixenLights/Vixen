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
		Pan, // Range

		[Description("Tilt")]
		Tilt, // Range

		[Description("Zoom")]
		Zoom, // Range

		[Description("Dim")]
		Dim, // Range

		[Description("Shutter")]
		Shutter, // Index

		[Description("Gobo")]
		Gobo, // Index

		[Description("Frost")]
		Frost, // Range

		[Description("Prism")]
		Prism, // Index

		[Description("Open Close Prism")]
		OpenClosePrism, // Index

		[Description("Spin Color Wheel")]
		SpinColorWheel, // Index
		
		[Description(" ")]
		Custom,
	}
}
