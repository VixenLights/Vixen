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
		/// Gets or sets a value that indicates whether missing output values emit the resting output values.
		/// </summary>
		/// <value><see langword="true" /> to emit resting values when no input produces an output; otherwise, <see langword="false" />. The default is <see langword="false" />.</value>
		[DataMember]
		public bool EnableDefaultValueMapping { get; set; } = false;

		/// <summary>
		/// Gets or sets the high-byte value emitted when no input produces an output.
		/// </summary>
		/// <value>The coarse output value. The default is <c>0</c>.</value>
		[DataMember]
		public byte RestingCoarseValue { get; set; } = 0;

		/// <summary>
		/// Gets or sets the low-byte value emitted when no input produces an output.
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
				RestingCoarseValue = RestingCoarseValue,
				RestingFineValue = RestingFineValue
			};
		}
	}
}
