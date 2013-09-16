using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Vixen.Module;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using Common.Controls.ColorManagement.ColorModels;

namespace VixenModules.Effect.Wipe {
	[DataContract]
	public class WipeData : ModuleDataModelBase {

		public WipeData() {
			Curve = new Curve();
			Curve.Points.Clear();
			Curve.Points.Add(99, 1);
			Curve.Points.Add(1, 99);
		 			
			Direction = WipeDirection.Right;
			ColorGradient = null;
			PulseTime = 1000;
			WipeByCount = false;
			PassCount = 0;
			PulsePercent = 33;
		}

		[DataMember]
		public ColorGradient ColorGradient { get; set; }

		[DataMember]
		public WipeDirection Direction{ get; set; }
		
		[DataMember]
		public Curve Curve { get; set; }

		[DataMember]
		public int PulseTime { get; set; }

		[DataMember]
		public bool WipeByCount { get; set; }

		[DataMember]
		public int PassCount { get; set; }

		[DataMember]
		public double PulsePercent { get; set; }
 
		public override IModuleDataModel Clone() {
			return (WipeData)MemberwiseClone();
		}
	}

	
}
