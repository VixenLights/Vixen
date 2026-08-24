using System.Runtime.Serialization;
using Vixen.Module;

namespace VixenModules.OutputFilter.CoarseFineBreakdown
{
	/// <summary>
	/// Stores the persisted configuration for the coarse/fine breakdown filter.
	/// </summary>
	public class CoarseFineBreakdownData : ModuleDataModelBase
	{
		/// <summary>
		/// Gets or sets a value that indicates whether the configured default input value is mapped to the resting output values.
		/// </summary>
		/// <value><see langword="true" /> to map the configured default input value; otherwise, <see langword="false" />. The default is <see langword="false" />.</value>
		[DataMember]
		public bool EnableDefaultValueMapping { get; set; } = false;

		/// <summary>
		/// Gets or sets the 16-bit input value that is mapped when default value mapping is enabled.
		/// </summary>
		/// <value>The input value to map. The default is <c>0</c>.</value>
		[DataMember]
		public ushort DefaultInputValue { get; set; } = 0;

		/// <summary>
		/// Gets or sets the high-byte value emitted for a mapped default input value.
		/// </summary>
		/// <value>The coarse output value. The default is <c>0</c>.</value>
		[DataMember]
		public byte RestingCoarseValue { get; set; } = 0;

		/// <summary>
		/// Gets or sets the low-byte value emitted for a mapped default input value.
		/// </summary>
		/// <value>The fine output value. The default is <c>0</c>.</value>
		[DataMember]
		public byte RestingFineValue { get; set; } = 0;

		/// <summary>
		/// Creates a copy of this output-filter configuration.
		/// </summary>
		/// <returns>A copy containing the current configuration values.</returns>
		public override IModuleDataModel Clone()
		{
			return new CoarseFineBreakdownData
			{
				EnableDefaultValueMapping = EnableDefaultValueMapping,
				DefaultInputValue = DefaultInputValue,
				RestingCoarseValue = RestingCoarseValue,
				RestingFineValue = RestingFineValue
			};
		}
	}
}
