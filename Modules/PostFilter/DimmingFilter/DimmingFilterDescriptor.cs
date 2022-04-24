using System;
using VixenModules.OutputFilter.TaggedFilter;

namespace VixenModules.OutputFilter.DimmingFilter
{
    /// <summary>
    /// Descriptor for the Shutter Filter module.
    /// </summary>
    public class DimmingFilterDescriptor : TaggedFilterDescriptorBase<DimmingFilterModule, DimmingFilterData>
	{
		#region Private Static Fields
		
		/// <summary>
		/// GUID associated with the type.
		/// </summary>
		private static readonly Guid _typeId = new Guid("{61913688-EF44-473C-91D4-D256CCA6CB91}");

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
		public override string TypeName => "Dimming Filter";

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public override Guid TypeId => _typeId;

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public override string Description => "An output filter that converts color intents into dimmer intents.";

		#endregion
	}
}