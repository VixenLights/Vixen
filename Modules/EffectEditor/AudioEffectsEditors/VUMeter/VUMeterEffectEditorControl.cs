using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using VixenModules.Property.Color;
using VixenModules.Effect.AudioHelp;
using Vixen.Sys;
using Vixen.Module.EffectEditor;
using Vixen.Module.Effect;
using VixenModules.Effect.VerticalMeter;
using Common.ValueTypes;
using VixenModules.App.ColorGradients;
using System.Drawing.Drawing2D;

namespace VixenModules.EffectEditor.AudioMeterEffectEditor
{
    public partial class VUMeterEffectEditorControl : AudioMeterEffectEditorControlBase
    {
        public VUMeterEffectEditorControl()
        {
            InitializeComponent();
        }

        public override object[] EffectParameterValues
        {
            get
            {
                return new object[]
				       	{
                            DecayTime,
                            AttackTime,
                            Normalize,
                            Gain,
                            Range,
                            GreenPos,
                            RedPos,
                            ColorGradientValue,
                            MeterColorStyle
				       	};
            }
            set
            {
                DecayTime = (int)value[0];
                AttackTime = (int)value[1];
                Normalize = (bool)value[2];
                Gain = (int)value[3];
                Range = (int)value[4];
                GreenPos = (int)value[5];
                RedPos = (int)value[6];
                ColorGradientValue = (ColorGradient)value[7];
                MeterColorStyle = (MeterColorTypes)value[8];
            }
        }
    }
}
