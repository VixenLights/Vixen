using System.Collections.Generic;
using System.Runtime.Serialization;
using Vixen.Module;

namespace VixenModules.OutputFilter.ColorBreakdown
{
	public class ColorBreakdownData : ModuleDataModelBase
	{
		public ColorBreakdownData()
		{
			BreakdownItems = new List<ColorBreakdownItem>();
		}

		public override IModuleDataModel Clone()
		{
			ColorBreakdownData newInstance = new ColorBreakdownData();
			newInstance.BreakdownItems = new List<ColorBreakdownItem>(BreakdownItems);
			newInstance.MixColors = MixColors;
			newInstance._16Bit = _16Bit;
			return newInstance;
		}

		[DataMember]
		public List<ColorBreakdownItem> BreakdownItems { get; set; }

		[DataMember]
		public bool MixColors { get; set; }

		/// <summary>
		/// Normalized color breakdown outputs range intents in the range of 0-1.
		/// </summary>
		[DataMember]
		public bool _16Bit { get; set; }
	}
}
