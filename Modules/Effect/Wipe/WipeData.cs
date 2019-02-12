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
			Curve = new Curve();
			Curve.Points.Clear();
			Curve.Points.Add(0, 0);
		 	Curve.Points.Add(50, 100);
			Curve.Points.Add(100, 0);
		 			
			Direction = WipeDirection.Horizontal;
			ColorGradient = new ColorGradient(Color.White);
			PulseTime = 1000;
			//WipeByCount = true;
			PassCount = 1;
			PulsePercent = 33;
			MovementCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 0.0, 100.0 }));
			WipeMovement = WipeMovement.Count;
			ReverseDirection = false;
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

		[DataMember(EmitDefaultValue = false)]
		public bool WipeByCount { get; set; }

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

		[OnDeserialized]
		public void OnDeserialized(StreamingContext c)
		{
			//Ensure defaults for new fields that might not be in older effects.
			if (MovementCurve == null)
			{
				MovementCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 0.0, 100.0 }));

			}
			if (!WipeByCount)
			{
				WipeMovement = WipeMovement.PulseLength;
				WipeByCount = true;
			}
		}

		protected override EffectTypeModuleData CreateInstanceForClone()
		{
			return (WipeData)MemberwiseClone();
		}
	}

	
}
