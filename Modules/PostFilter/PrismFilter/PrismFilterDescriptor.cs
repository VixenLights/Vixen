using System;
using VixenModules.OutputFilter.TaggedFilter;

namespace VixenModules.OutputFilter.PrismFilter
{
    /// <summary>
    /// Descriptor for the Prism Filter module.
    /// </summary>
    public class PrismFilterDescriptor : TaggedFilterDescriptorBase<PrismFilterModule, PrismFilterData>
	{
		#region Private Static Fields

		/// <summary>
		/// GUID associated with the type.
		/// </summary>
		private static readonly Guid _typeId = new Guid("{2522BD94-0434-4728-9400-0E18B212DC21}");

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
		public override string TypeName => "Prism Filter";

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public override Guid TypeId => _typeId;

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public override string Description => "An output filter that converts prism intents into open prism commands.";

		#endregion
	}
}