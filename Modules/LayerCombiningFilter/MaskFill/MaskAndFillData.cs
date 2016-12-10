using System.Runtime.Serialization;
using Vixen.Module;

namespace VixenModules.LayerMixingFilter.MaskFill
{
	public class MaskAndFillData: ModuleDataModelBase
	{
		public MaskAndFillData()
		{
			ExcludeZeroValues = true;
		}

		public override IModuleDataModel Clone()
		{
			MaskAndFillData newInstance = new MaskAndFillData {ExcludeZeroValues = ExcludeZeroValues};
			return newInstance;
		}

		[DataMember]
		public bool ExcludeZeroValues { get; set; }
		
	}
}
