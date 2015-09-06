using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Vixen.Module;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using System.Drawing;
using System.Drawing.Drawing2D;
using Vixen.Module.Effect;
using VixenModules.Effect.AudioHelp;

namespace VixenModules.Effect.VUMeter
{
	[DataContract]
	public class VUMeterData : ModuleDataModelBase, IAudioPluginData
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

            /*
            Color[] myColors = { Color.Lime, Color.Yellow, Color.Red };
            float[] myPositions = { (float)0.00000000000001, (float)GreenColorPosition / 100, (float)RedColorPosition / 100 };
            ColorBlend linearBlend = new ColorBlend();
            linearBlend.Colors = myColors;
            linearBlend.Positions = myPositions;*/

            Color[] myColors = { Color.White, Color.White };
            float[] myPositions = { (float)0.00000000000001, (float)98 / 100 };
            ColorBlend linearBlend = new ColorBlend();
            linearBlend.Colors = myColors;
            linearBlend.Positions = myPositions;

            IntensityCurve = new Curve();
            IntensityCurve.Points.Clear();
            IntensityCurve.Points.Add(new ZedGraph.PointPair(0, 0));
            IntensityCurve.Points.Add(new ZedGraph.PointPair(100, 100));

            ColorGradient linearGradient = new ColorGradient(linearBlend);
            MeterColorGradient = linearGradient;
        }

        public override IModuleDataModel Clone()
        {
            VUMeterData result = new VUMeterData();
            result.DecayTime = DecayTime;
            result.AttackTime = AttackTime;
            result.Normalize = Normalize;
            result.Gain = Gain;
            result.Range = Range;
            result.GreenColorPosition = GreenColorPosition;
            result.RedColorPosition = RedColorPosition;
            result.MeterColorGradient = MeterColorGradient;

            return result;
        }
	}
}