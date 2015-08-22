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

namespace VixenModules.Effect.AudioHelp
{
    public interface IAudioPluginData : IModuleDataModel
    {
        int DecayTime { get; set; }
        int AttackTime { get; set; }
        bool Normalize { get; set; }
        int Gain { get; set; }
        int Range { get; set; }
        int GreenColorPosition { get; set; }
        int RedColorPosition { get; set; }
        bool LowPass { get; set; }
        int LowPassFreq { get; set; }
        bool HighPass { get; set; }
        int HighPassFreq { get; set; }
        ColorGradient MeterColorGradient { get; set; }
        MeterColorTypes MeterColorStyle { get; set; }
    }
}