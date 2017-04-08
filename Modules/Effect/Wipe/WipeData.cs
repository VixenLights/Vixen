using System.Runtime.Serialization;
using Vixen.Module;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using System.Drawing;
using VixenModules.Effect.Effect;

namespace VixenModules.Effect.Wipe {
	[DataContract]
	public class WipeData : EffectTypeModuleData {

		public WipeData() {
			Curve = new Curve();
			Curve.Points.Clear();
			Curve.Points.Add(0, 0);
		 	Curve.Points.Add(50, 100);
			Curve.Points.Add(100, 0);
		 			
			Direction = WipeDirection.Right;
			ColorGradient = new ColorGradient(Color.White);
			PulseTime = 1000;
			WipeByCount = false;
			PassCount = 1;
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

		[DataMember]
		public bool WipeOn { get; set; }

		[DataMember]
		public bool WipeOff { get; set; }
		
		protected override EffectTypeModuleData CreateInstanceForClone()
		{
			return (WipeData)MemberwiseClone();
		}
	}

	
}
