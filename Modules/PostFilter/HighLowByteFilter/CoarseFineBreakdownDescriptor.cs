using System;
using Vixen.Module.OutputFilter;

namespace VixenModules.OutputFilter.CoarseFineBreakdown
{
	/// <summary>
	/// Descriptor for the module.
	/// </summary>
	public class CoarseFineBreakdownDescriptor : OutputFilterModuleDescriptorBase
	{
		#region Static Fields
		
		/// <summary>
		/// Unique ID for the module.
		/// </summary>
		private static readonly Guid _typeId = new Guid("{7C1465DF-054C-4875-AB61-D7E7F5236897}");

		#endregion

		#region IModuleDescriptor

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public override string TypeName => "Coarse Fine Breakdown";

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public override Guid TypeId => _typeId;

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public static Guid ModuleId => _typeId;

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public override Type ModuleClass => typeof (CoarseFineBreakdownModule);

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public override Type ModuleDataClass => typeof (CoarseFineBreakdownData);

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public override string Author => "Vixen Team";

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public override string Description => "An output filter that breaks an intent value into a coarse (high byte) and a fine (low byte).";

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public override string Version => "1.0";

		#endregion
	}
}