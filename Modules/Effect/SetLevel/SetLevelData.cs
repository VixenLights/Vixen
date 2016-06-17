using System.Runtime.Serialization;
using System.Drawing;
using Common.Controls.ColorManagement.ColorModels;
using VixenModules.Effect.Effect;

namespace VixenModules.Effect.SetLevel
{
	[DataContract]
	internal class SetLevelData : EffectTypeModuleData
	{
		[DataMember]
		public double level { get; set; }

		[DataMember]
		public RGB color { get; set; }

		public SetLevelData()
		{
			level = 1;
			color = Color.White;
		}

		protected override EffectTypeModuleData CreateInstanceForClone()
		{
			SetLevelData result = new SetLevelData();
			result.level = level;
			result.color = color;
			return result;
		}
	}
}