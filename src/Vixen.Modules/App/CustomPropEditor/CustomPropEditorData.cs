using System.Drawing;
using System.Runtime.Serialization;
using Vixen.Module;
using VixenModules.App.CustomPropEditor.Model;

namespace VixenModules.App.CustomPropEditor
{
	[DataContract]
	public class CustomPropEditorData : ModuleDataModelBase
	{
		private ModuleLocalDataSet _moduleData;

		public override IModuleDataModel Clone()
		{
			CustomPropEditorData newInstance = new CustomPropEditorData();
			newInstance.ModuleData = ModuleData.Clone() as ModuleLocalDataSet;
			return newInstance;
		}



		[DataMember]
		public ModuleLocalDataSet ModuleData
		{
			get { return _moduleData ?? (_moduleData = new ModuleLocalDataSet()); }
			set { _moduleData = value; }
		}

		[DataMember]
		public Color LightColor { get; set; } = Configuration.DefaultLightColor;

		[DataMember]
		public Color SelectedLightColor { get; set; } = Configuration.DefaultSelectedLightColor;
		
		[DataMember]
		public Color StatePreviewBaseColor { get; set; } = Configuration.DefaultStatePreviewBaseColor;

		[DataMember]
		public uint DefaultLightSize { get; set; } = ElementModel.DefaultLightSize;

		[OnDeserialized]
		public void OnDeserialized(StreamingContext c)
		{
			if (LightColor == Color.Empty)
			{
				LightColor = Configuration.DefaultLightColor;
			}

			if (SelectedLightColor == Color.Empty)
			{
				SelectedLightColor = Configuration.DefaultSelectedLightColor;
			}
			
			if (StatePreviewBaseColor == Color.Empty)
			{
				StatePreviewBaseColor = Configuration.DefaultStatePreviewBaseColor;
			}

			if (DefaultLightSize == 0)
			{
				DefaultLightSize = ElementModel.DefaultLightSize;
			}
		}
	}
}