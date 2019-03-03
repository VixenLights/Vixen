using System.Runtime.Serialization;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using System.Drawing;
using VixenModules.Effect.Effect;
using ZedGraph;

namespace VixenModules.Effect.Wipe {
	[DataContract]
	public class WipeData : EffectTypeModuleData {
		
		public WipeData() {
			Curve = new Curve(new PointPairList(new[] { 0.0, 50.0, 100.0 }, new[] { 0.0, 100.0, 0.0 }));
			Direction = WipeDirection.Horizontal;
			ColorGradient = new ColorGradient(Color.White);
			PulseTime = 1000;
			PassCount = 1;
			PulsePercent = 33;
			MovementCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 0.0, 100.0 }));
			WipeMovement = WipeMovement.Count;
			ReverseDirection = false;
			ColorHandling = ColorHandling.GradientThroughWholeEffect;
			WipeOn = false;
			WipeOff = false;
			ColorAcrossItemPerCount = true;
			ReverseColorDirection = true;
		}
		[DataMember]
		public ColorHandling ColorHandling { get; set; }

		[DataMember]
		public ColorGradient ColorGradient { get; set; }

		[DataMember]
		public WipeDirection Direction{ get; set; }
		
		[DataMember]
		public Curve Curve { get; set; }

		[DataMember]
		public int PulseTime { get; set; }

		[DataMember]
		public int PassCount { get; set; }

		[DataMember]
		public double PulsePercent { get; set; }
		
		[DataMember]
		public bool WipeOn { get; set; }

		[DataMember]
		public bool WipeOff { get; set; }
		
		[DataMember]
		public Curve MovementCurve { get; set; }

		[DataMember]
		public WipeMovement WipeMovement { get; set; }

		[DataMember] 
		public bool ReverseDirection { get; set; }

		[DataMember]
		public bool ColorAcrossItemPerCount { get; set; }

		[DataMember]
		public bool ReverseColorDirection { get; set; }

		protected override EffectTypeModuleData CreateInstanceForClone()
		{
			return (WipeData)MemberwiseClone();
		}
	}

	
}
