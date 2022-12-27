using System.Runtime.Serialization;
using Vixen.Module;

namespace VixenModules.Property.Color
{
	[DataContract]
	public class ColorData : ModuleDataModelBase
	{
		[DataMember]
		public ElementColorType ElementColorType { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public System.Drawing.Color SingleColor { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public string ColorSetName { get; set; }

		public ColorData()
		{
			ElementColorType = ElementColorType.FullColor;
			SingleColor = System.Drawing.Color.Empty;
			ColorSetName = null;
		}

		public override IModuleDataModel Clone()
		{
			return (ColorData) MemberwiseClone();
		}

		[OnDeserialized]
		private void OnDeserialized(StreamingContext context)
		{
			if(ElementColorType == ElementColorType.FullColor)
			{
				//Set any previous ones to the default so they won't be serialized needlessly
				SingleColor = System.Drawing.Color.Empty;
			}
			if (ColorSetName == String.Empty)
			{
				ColorSetName = null;
			}
		}

	}
}