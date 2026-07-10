using System.Runtime.Serialization;
using Vixen.Module;

namespace VixenModules.Output.DummyLighting
{
	[DataContract]
	internal class DummyLightingData : ModuleDataModelBase
	{
		[DataMember]
		public RenderStyle RenderStyle { get; set; }

		[DataMember]
		public string FormTitle { get; set; }

		public DummyLightingData()
		{
			RenderStyle = RenderStyle.Monochrome;
			FormTitle = "Dummy Lighting Controller";
		}

		public override IModuleDataModel Clone()
		{
			DummyLightingData result = new DummyLightingData();
			result.RenderStyle = RenderStyle;
			result.FormTitle = FormTitle;
			return result;
		}
	}
}