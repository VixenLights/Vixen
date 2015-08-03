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

namespace VixenModules.Effect.VerticalMeter
{
	[DataContract]
	public class VerticalMeterData : ModuleDataModelBase, IAudioPluginData
	{
        [DataMember]
        public bool Inverted { get; set; }

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

        public VerticalMeterData()
        {
            Inverted = false;
            DecayTime = 1500;
            AttackTime = 50;
            Normalize = true;
            Gain = 5;
            Range = 20;
            RedColorPosition = 95;
            GreenColorPosition = 25;
            MeterColorStyle = MeterColorTypes.Linear;

            Color[] myColors = { Color.Lime, Color.Yellow, Color.Red };
            float[] myPositions = { (float)0.00000000000001, (float)GreenColorPosition / 100, (float)RedColorPosition / 100 };
            ColorBlend linearBlend = new ColorBlend();
            linearBlend.Colors = myColors;
            linearBlend.Positions = myPositions;

            ColorGradient linearGradient = new ColorGradient(linearBlend);
            MeterColorGradient = linearGradient;
        }

        public override IModuleDataModel Clone()
        {
            VerticalMeterData result = new VerticalMeterData();
            result.Inverted = Inverted;
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