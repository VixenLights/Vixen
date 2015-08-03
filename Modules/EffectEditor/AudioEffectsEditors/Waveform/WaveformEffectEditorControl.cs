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
    public partial class WaveformEffectEditorControl : AudioMeterEffectEditorControlBase
    {
        public WaveformEffectEditorControl()
        {
            InitializeComponent();
        }

        public bool ReverseEFfect { get { return (bool)(ReverseCheckbox.Checked); } set { ReverseCheckbox.Checked = value; } }
        public int ScrollSpeed { get { return (int)ScroolSpeedTrackBar.Value; } set { ScroolSpeedTrackBar.Value = value; } }

        public override object[] EffectParameterValues
        {
            get
            {
                return new object[]
				       	{
                            ReverseEFfect,
                            ScrollSpeed,
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
                ReverseEFfect = (bool)value[0];
                ScrollSpeed = (int)value[1];
                DecayTime = (int)value[2];
                AttackTime = (int)value[3];
                Normalize = (bool)value[4];
                Gain = (int)value[5];
                Range = (int)value[6];
                GreenPos = (int)value[7];
                RedPos = (int)value[8];
                ColorGradientValue = (ColorGradient)value[9];
                MeterColorStyle = (MeterColorTypes)value[10];
            }
        }
    }
}
