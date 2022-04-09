using System;
using Vixen.Module.OutputFilter;

namespace VixenModules.OutputFilter.TaggedFilter
{
    /// <summary>
    /// Asbract base class for a tagged filter descriptor.
    /// </summary>
    /// <typeparam name="TFilterModule">Type of the Tagged filter module</typeparam>
    /// <typeparam name="TFilterData">Type of the Tagged filter data</typeparam>
    public abstract class TaggedFilterDescriptorBase<TFilterModule, TFilterData> : OutputFilterModuleDescriptorBase
	{
		#region IModuleDescriptor

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public override string TypeName => "Tag Filter";

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public override Type ModuleClass => typeof(TFilterModule);

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public override Type ModuleDataClass => typeof(TFilterData);

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public override string Author => "Vixen Team";

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public override string Version => "1.0";
		
		#endregion
	}
}
