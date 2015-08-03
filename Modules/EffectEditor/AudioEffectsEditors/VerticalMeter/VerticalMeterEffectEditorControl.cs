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
    public partial class VerticalMeterEffectEditorControl : AudioMeterEffectEditorControlBase
    {
        public VerticalMeterEffectEditorControl()
        {
            InitializeComponent();
        }

        public bool ReverseEFfect { get { return (bool)(ReverseCheckbox.Checked); } set { ReverseCheckbox.Checked = value; } }

        public override object[] EffectParameterValues
        {
            get
            {
                return new object[]
				       	{
                            ReverseCheckbox.Checked,
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
                ReverseCheckbox.Checked = (bool)value[0];
                DecayTime = (int)value[1];
                AttackTime = (int)value[2];
                Normalize = (bool)value[3];
                Gain = (int)value[4];
                Range = (int)value[5];
                GreenPos = (int)value[6];
                RedPos = (int)value[7];
                ColorGradientValue = (ColorGradient)value[8];
                MeterColorStyle = (MeterColorTypes)value[9];
            }
        }
    }
}
