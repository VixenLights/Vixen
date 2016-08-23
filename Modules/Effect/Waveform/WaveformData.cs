using System.Runtime.Serialization;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using System.Drawing;
using System.Drawing.Drawing2D;
using VixenModules.Effect.AudioHelp;
using VixenModules.Effect.Effect;

namespace VixenModules.Effect.Waveform
{
	[DataContract]
	public class WaveformData : EffectTypeModuleData, IAudioPluginData
	{
        [DataMember]
        public bool Inverted { get; set; }

        [DataMember]
        public int ScrollSpeed { get; set; }

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
        public Curve IntensityCurve { get; set; }

        [DataMember]
        public MeterColorTypes MeterColorStyle {get; set;}

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

		public WaveformData()
        {
            Inverted = false;
            ScrollSpeed = 10;
            DecayTime = 1500;
            AttackTime = 50;
            Normalize = true;
            Gain = 0;
            Range = 10;
            RedColorPosition = 95;
            GreenColorPosition = 25;
            MeterColorStyle = MeterColorTypes.Linear;

            LowPass = false;
            LowPassFreq = 100;
            HighPass = false;
            HighPassFreq = 500;

            Color[] myColors = { Color.Lime, Color.Yellow, Color.Red };
            float[] myPositions = { 0, (float)GreenColorPosition / 100, (float)RedColorPosition / 100 };
            ColorBlend linearBlend = new ColorBlend();
            linearBlend.Colors = myColors;
            linearBlend.Positions = myPositions;

            IntensityCurve = new Curve();
            IntensityCurve.Points.Clear();
            IntensityCurve.Points.Add(new ZedGraph.PointPair(0, 100));
            IntensityCurve.Points.Add(new ZedGraph.PointPair(100, 100));

            ColorGradient linearGradient = new ColorGradient(linearBlend);
            MeterColorGradient = linearGradient;

			DepthOfEffect = 0;
        }

		protected override EffectTypeModuleData CreateInstanceForClone()
		{
			WaveformData result = new WaveformData();
			result.HighPass = HighPass;
			result.LowPass = LowPass;
			result.HighPassFreq = HighPassFreq;
			result.LowPassFreq = LowPassFreq;
			result.Inverted = Inverted;
			result.ScrollSpeed = ScrollSpeed;
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