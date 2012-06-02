using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Vixen.Module;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;

namespace ColorWithIntent {
	[DataContract]
	public class ColorData : ModuleDataModelBase {
		public ColorData() {
			LevelCurve = new Curve();
			ColorGradient = new ColorGradient();
		}

		public override IModuleDataModel Clone() {
			return (ColorData)MemberwiseClone();
		}

		[DataMember]
		public Curve LevelCurve { get; set; }

		[DataMember]
		public ColorGradient ColorGradient { get; set; }


	}
}
