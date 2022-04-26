using System;
using VixenModules.OutputFilter.TaggedFilter;

namespace VixenModules.OutputFilter.ColorWheelFilter
{
    /// <summary>
    /// Descriptor for the Color Wheel Filter module.
    /// </summary>
    public class ColorWheelFilterDescriptor : TaggedFilterDescriptorBase<ColorWheelFilterModule, ColorWheelFilterData>
	{
		#region Private Static Fields
		
		/// <summary>
		/// GUID associated with the type.
		/// </summary>
		private static readonly Guid _typeId = new Guid("{18470C0C-1E75-464F-BD9F-A3E241CD1EB8}");

		#endregion

		#region Public Static Properties

		/// <summary>
		/// Module ID for the filter.
		/// </summary>
		public static Guid ModuleId => _typeId;

		#endregion

		#region IModuleDescriptor

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public override string TypeName => "Color Wheel Filter";

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public override Guid TypeId => _typeId;

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public override string Description => "An output filter that converts color intents into color wheel index commands.";

		#endregion
	}
}