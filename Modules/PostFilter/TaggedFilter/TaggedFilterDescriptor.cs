using System;

namespace VixenModules.OutputFilter.TaggedFilter
{
	/// <summary>
	/// Descriptor for the Tagged Filter module.
	/// </summary>
	public class TaggedFilterDescriptor : TaggedFilterDescriptorBase<TaggedFilterModule, TaggedFilterData>
	{
		#region Private Static Properties

		/// <summary>
		/// GUID associated with descriptor.
		/// </summary>
		private static readonly Guid _typeId = new Guid("{FE552DCF-1BC0-4588-B23C-2600151D9FC5}");

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
		public override Guid TypeId => _typeId;

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public override string Description => "An output filter that filters intents based on a tag.";

		#endregion
	}
}