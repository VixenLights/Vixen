using System.Runtime.Serialization;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using System.Drawing;
using VixenModules.Effect.AudioHelp;
using VixenModules.Effect.Effect;

namespace VixenModules.Effect.VUMeter
{
	[DataContract]
	public class VUMeterData : EffectTypeModuleData, IAudioPluginData
	{
        [DataMember]
        public int DecayTime { get; set; }

        [DataMember]
        public int AttackTime { get; set; }

        [DataMember]
        public bool Normalize { get; set; }

        [DataMember]
        public int Gain { get; set; }

        [DataMember]
        public int Range { get; set; }

        [DataMember]
        public int GreenColorPosition { get; set; }

        [DataMember]
        public int RedColorPosition { get; set; }

        [DataMember]
        public ColorGradient MeterColorGradient { get; set; }

        [DataMember]
        public MeterColorTypes MeterColorStyle {get; set;}

        [DataMember]
        public Curve IntensityCurve { get; set; }

        [DataMember]
        public bool LowPass { get; set; }

        [DataMember]
        public int LowPassFreq { get; set; }

        [DataMember]
        public bool HighPass { get; set; }

        [DataMember]
        public int HighPassFreq { get; set; }

		[DataMember]
		public int DepthOfEffect { get; set; }

		public VUMeterData()
        {
            DecayTime = 1500;
            AttackTime = 50;
            Normalize = true;
            Gain = 0;
            Range = 10;
            RedColorPosition = 95;
            GreenColorPosition = 25;
            MeterColorStyle = MeterColorTypes.Custom;

            LowPass = false;
            LowPassFreq = 100;
            HighPass = false;
            HighPassFreq = 500;

			IntensityCurve = new Curve();
            IntensityCurve.Points.Clear();
            IntensityCurve.Points.Add(new ZedGraph.PointPair(0, 0));
            IntensityCurve.Points.Add(new ZedGraph.PointPair(100, 100));

            ColorGradient linearGradient = new ColorGradient(Color.White);
            MeterColorGradient = linearGradient;

			DepthOfEffect = 0;
		}

		protected override EffectTypeModuleData CreateInstanceForClone()
		{
			VUMeterData result = new VUMeterData();
			result.HighPass = HighPass;
			result.LowPass = LowPass;
			result.HighPassFreq = HighPassFreq;
			result.LowPassFreq = LowPassFreq;
			result.DecayTime = DecayTime;
			result.AttackTime = AttackTime;
			result.Normalize = Normalize;
			result.Gain = Gain;
			result.Range = Range;
			result.GreenColorPosition = GreenColorPosition;
			result.RedColorPosition = RedColorPosition;
			result.MeterColorGradient = new ColorGradient(MeterColorGradient);
			result.IntensityCurve = new Curve(IntensityCurve);
			result.DepthOfEffect = DepthOfEffect;

			return result;
		}
	}
}