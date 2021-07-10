using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using Vixen.Module;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.Effect.DMXFixture;
using VixenModules.Effect.Effect;
using ZedGraph;

namespace VixenModules.Effect.Bars
{
	[DataContract]
	public class DMXFixtureData: EffectTypeModuleData
	{

		public DMXFixtureData()
		{
			BeamColor = new ColorGradient(Color.Yellow);
			LegendColor = new ColorGradient(Color.Red);
			Pan = 0;
			Tilt = 0;
			Intensity = 100;
			BeamLength = 1;
            OnOff = true;
            Orientation = StringOrientation.Vertical;
            Focus = 100;
            Gobo = Gobos.Open;
            IncludeLegend = true;			
		}

		[DataMember]
		public ColorGradient BeamColor { get; set; }

		[DataMember]
		public ColorGradient LegendColor { get; set; }

		[DataMember]
		public int Pan { get; set; }

		[DataMember]
		public int Tilt { get; set; }

		[DataMember]
		public int BeamLength { get; set; }

		[DataMember]
		public bool OnOff { get; set; }

		[DataMember] 
		public int Intensity { get; set; }

		[DataMember]
		public int Focus { get; set; }

		[DataMember]
		public StringOrientation Orientation { get; set; }

		[DataMember]
		public Gobos Gobo { get; set; }

		[DataMember]
		public PrismType Prism { get; set; }

		[DataMember]
		public bool IncludeLegend { get; set; }

		[DataMember]
		public bool EnableGDI { get; set; }
		
		[OnDeserialized]
		public void OnDeserialized(StreamingContext c)
		{
		}

		protected override EffectTypeModuleData CreateInstanceForClone()
		{
			DMXFixtureData result = new DMXFixtureData
			{
				BeamColor = BeamColor,
				LegendColor = LegendColor,
				Pan = Pan,
				Tilt = Tilt,
				OnOff = OnOff,
				BeamLength = BeamLength,
				Intensity = Intensity,
				Gobo = Gobo,
				Prism = Prism,
				Orientation = Orientation,
				Focus = Focus,
				EnableGDI = EnableGDI				
			};
			return result;
		}
	}
}
