using System.ComponentModel;
using Vixen.TypeConverters;

namespace Vixen.Commands
{
    /// <summary>
    /// Defines the types of indices to support the preview.
    /// </summary>
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
	public enum FixtureIndexType
	{
		/// <summary>
		/// Custom index type.
		/// </summary>
		[Description(" ")]
		Custom,

		/// <summary>
		/// Index command to open the fixture shutter.
		/// </summary>
		[Description("Shutter Open")]
		ShutterOpen,

		/// <summary>
		/// Index command to close the fixture shutter.
		/// </summary>
		[Description("Shutter Closed")]
		ShutterClosed,

		/// <summary>
		/// Index command to turn a fixture lamp on.
		/// </summary>
		[Description("Lamp On")]
		LampOn,

		/// <summary>
		/// Index command to turn a fixture lamp off.
		/// </summary>
		[Description("Lamp Off")]
		LampOff,

		/// <summary>
		/// Index command that represents a color wheel item.
		/// </summary>
		[Description("Color Wheel")]
		ColorWheel,
	}
}
