using System.Drawing;
using System.Runtime.Serialization;
using VixenModules.App.ColorGradients;
using VixenModules.Effect.Effect;

namespace VixenModules.Effect.Shockwave
{
	[DataContract]
	public class ShockwaveData: EffectTypeModuleData
	{

		public ShockwaveData()
		{
			Gradient = new ColorGradient();
			Gradient.Colors.Clear();
			Gradient.Colors.Add(new ColorPoint(Color.Red,0.0));
			Gradient.Colors.Add(new ColorPoint(Color.Lime, .5));
			Gradient.Colors.Add(new ColorPoint(Color.Blue, 1.0));
			StartWidth = 5;
			EndWidth = 10;
			StartRadius = 1;
			EndRadius = 10;
			CenterX = 50;
			CenterY = 50;
			Acceleration = 0;
			BlendEdges = true;
			Orientation=StringOrientation.Vertical;
		}

		[DataMember]
		public ColorGradient Gradient { get; set; }

		[DataMember]
		public int CenterX { get; set; }

		[DataMember]
		public int CenterY { get; set; }

		[DataMember]
		public int StartRadius { get; set; }

		[DataMember]
		public int EndRadius { get; set; }

		[DataMember]
		public int StartWidth { get; set; }

		[DataMember]
		public int EndWidth { get; set; }

		[DataMember]
		public int Acceleration { get; set; }

		[DataMember]
		public bool BlendEdges { get; set; }

		[DataMember]
		public StringOrientation Orientation { get; set; }

		protected override EffectTypeModuleData CreateInstanceForClone()
		{
			ShockwaveData result = new ShockwaveData
			{
				
				Orientation = Orientation,
				Gradient = new ColorGradient(Gradient),
				StartWidth = StartWidth,
				EndWidth = EndWidth,
				StartRadius = StartRadius,
				EndRadius = EndRadius,
				CenterX = CenterX,
				CenterY = CenterY,
				Acceleration = Acceleration,
				BlendEdges = BlendEdges
			};
			return result;
		}
	}
}
