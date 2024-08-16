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
		public Color LightColor { get; set; } = Color.White;

		[DataMember]
		public Color SelectedLightColor { get; set; } = Color.HotPink;

		[DataMember]
		public uint DefaultLightSize { get; set; } = ElementModel.DefaultLightSize;

		[OnDeserialized]
		public void OnDeserialized(StreamingContext c)
		{
			if (LightColor == Color.Empty)
			{
				LightColor = Color.White;
			}

			if (SelectedLightColor == Color.Empty)
			{
				SelectedLightColor = Color.HotPink;
			}

			if (DefaultLightSize == 0)
			{
				DefaultLightSize = ElementModel.DefaultLightSize;
			}
		}
	}
}